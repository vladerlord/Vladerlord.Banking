using MassTransit;
using Npgsql;
using Service.Identity.Abstractions;
using Service.Identity.Models;
using Service.Identity.Services;
using Shared.Abstractions.Grpc.Identity;
using Shared.Abstractions.Grpc.Identity.Contracts;

namespace Service.Identity;

public class IdentityGrpcService : IIdentityGrpcService
{
	private readonly IUserRepository _userRepository;
	private readonly IConfirmationLinkRepository _confirmationLinkRepository;
	private readonly TokenAuthService _tokenAuthService;
	private readonly IPublishEndpoint _publishEndpoint;
	private readonly EncryptionService _encryptionService;

	public IdentityGrpcService(IUserRepository userRepository,
		IConfirmationLinkRepository confirmationLinkRepository,
		TokenAuthService tokenAuthService,
		IPublishEndpoint publishEndpoint,
		EncryptionService encryptionService)
	{
		_userRepository = userRepository;
		_confirmationLinkRepository = confirmationLinkRepository;
		_tokenAuthService = tokenAuthService;
		_publishEndpoint = publishEndpoint;
		_encryptionService = encryptionService;
	}

	public async Task<LoginGrpcResponse> LoginAsync(LoginGrpcRequest request)
	{
		var user = await _userRepository.FindByEmailAsync(request.Email);

		if (user == null)
			return new LoginGrpcResponse { Status = IdentityResponseStatus.NotFound };

		var isPasswordCorrect = _encryptionService.VerifyHash(request.Password, user.Password);

		if (!isPasswordCorrect)
			return new LoginGrpcResponse { Status = IdentityResponseStatus.NotFound };

		return new LoginGrpcResponse
		{
			Status = IdentityResponseStatus.Ok,
			Jwt = _tokenAuthService.GenerateToken(user.Email)
		};
	}

	public async Task<RegisterUserGrpcResponse> RegisterAsync(RegisterUserGrpcRequest request)
	{
		UserDatabaseModel? user = null;
		var status = IdentityResponseStatus.Ok;

		try
		{
			var hashedPassword = EncryptionService.Hash(request.Password);
			var iv = _encryptionService.GenerateIv();

			user = await _userRepository.CreateAsync(request.ToDatabaseModel(hashedPassword, iv, UserStatus.Created));

			await _confirmationLinkRepository.CreateAsync(new ConfirmationLinkDatabaseModel
			(
				Guid.NewGuid(),
				ConfirmationLinkType.RegistrationConfirmation,
				request.ConfirmationCode,
				user.Id
			));

			await _publishEndpoint.Publish(user.ToCreatedEvent(request.RegisterConfirmationUrl));
		}
		catch (PostgresException e)
		{
			// todo, check that field is email
			status = e.SqlState == PostgresErrorCodes.UniqueViolation
				? IdentityResponseStatus.UserAlreadyExist
				: IdentityResponseStatus.Error;
		}
		catch (Exception)
		{
			status = IdentityResponseStatus.Error;
		}

		return new RegisterUserGrpcResponse
		{
			Status = status,
			UserModel = user?.ToGrpcModel()
		};
	}

	public Task<VerifyTokenGrpcResponse> VerifyTokenAsync(VerifyTokenGrpcRequest request)
	{
		var result = _tokenAuthService.VerifyToken(request.JwtToken);

		return Task.FromResult(new VerifyTokenGrpcResponse
		{
			Status = result ? IdentityResponseStatus.Ok : IdentityResponseStatus.Invalid
		});
	}

	public async Task<RegisterConfirmationGrpcResponse> RegisterConfirmationAsync(
		RegisterConfirmationGrpcRequest request)
	{
		var confirmationLink = await _confirmationLinkRepository.FindByConfirmationCodeAsync(request.ConfirmationCode);

		if (confirmationLink == null)
			return new RegisterConfirmationGrpcResponse { Status = IdentityResponseStatus.NotFound };

		var user = await _userRepository.FindByIdAsync(confirmationLink.UserId);

		if (user == null)
			return new RegisterConfirmationGrpcResponse { Status = IdentityResponseStatus.NotFound };

		await _userRepository.UpdateStatusAsync(user.Id, UserStatus.Confirmed);
		await _confirmationLinkRepository.DeleteByIdAsync(confirmationLink.Id);

		return new RegisterConfirmationGrpcResponse
		{
			Status = IdentityResponseStatus.Ok
		};
	}
}