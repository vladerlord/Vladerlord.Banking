using System.Diagnostics;
using Chassis;
using MassTransit;
using Npgsql;
using Service.Identity.Abstractions;
using Service.Identity.Models;
using Service.Identity.Scheme;
using Service.Identity.Services;
using Shared.Abstractions;
using Shared.Grpc;
using Shared.Grpc.Identity;
using Shared.Grpc.Identity.Contracts;

namespace Service.Identity;

public class IdentityGrpcService : IIdentityGrpcService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfirmationLinkRepository _confirmationLinkRepository;
    private readonly TokenAuthService _tokenAuthService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly HashService _hashService;

    private static readonly ActivitySource Activity = new(Metrics.GetServiceName());

    public IdentityGrpcService(IUserRepository userRepository,
        IConfirmationLinkRepository confirmationLinkRepository,
        TokenAuthService tokenAuthService,
        IPublishEndpoint publishEndpoint,
        HashService hashService)
    {
        _userRepository = userRepository;
        _confirmationLinkRepository = confirmationLinkRepository;
        _tokenAuthService = tokenAuthService;
        _publishEndpoint = publishEndpoint;
        _hashService = hashService;
    }

    public async Task<LoginGrpcResponse> LoginAsync(LoginGrpcRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);

        if (user == null)
            return new LoginGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };

        var isPasswordCorrect = _hashService.VerifyHash(request.Password, user.Password);

        if (!isPasswordCorrect)
            return new LoginGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };

        return new LoginGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            Jwt = _tokenAuthService.GenerateToken(user),
            UserModel = user.ToGrpcModel()
        };
    }

    public async Task<RegisterUserGrpcResponse> RegisterAsync(RegisterUserGrpcRequest request)
    {
        UserDatabaseModel? user = null;
        var status = GrpcResponseStatus.Ok;

        try
        {
            var hashedPassword = _hashService.Hash(request.Password);

            user = await _userRepository.CreateAsync(request.ToDatabaseModel(hashedPassword, UserStatus.Created));

            await _confirmationLinkRepository.CreateAsync(new ConfirmationLinkDatabaseModel
            (
                Guid.NewGuid(),
                ConfirmationLinkType.RegistrationConfirmation,
                request.ConfirmationCode,
                user.Id
            ));

            using var activity = Activity.StartActivity(ActivityKind.Producer);
            activity?.SetTag("email", request.Email);

            await _publishEndpoint.Publish(user.ToCreatedEvent(request.RegisterConfirmationUrl, activity?.Id));
        }
        catch (PostgresException e)
        {
            var emailViolation = e.SqlState == PostgresErrorCodes.UniqueViolation &&
                                 e.ConstraintName?.Contains(UserDatabaseSchema.Columns.Email) == true;

            status = emailViolation ? GrpcResponseStatus.UserAlreadyExist : GrpcResponseStatus.Error;
        }
        catch (Exception)
        {
            status = GrpcResponseStatus.Error;
        }

        return new RegisterUserGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = status },
            UserModel = user?.ToGrpcModel()
        };
    }

    public Task<VerifyTokenGrpcResponse> VerifyTokenAsync(VerifyTokenGrpcRequest request)
    {
        var result = _tokenAuthService.VerifyToken(request.JwtToken);

        var claims = result?.Claims
            .Select(resultClaim => new KeyValuePair<string, string>(resultClaim.Type, resultClaim.Value))
            .ToList();

        return Task.FromResult(new VerifyTokenGrpcResponse
        {
            Status = result != null ? GrpcResponseStatus.Ok : GrpcResponseStatus.Invalid,
            Claims = claims
        });
    }

    public async Task<RegisterConfirmationGrpcResponse> RegisterConfirmationAsync(
        RegisterConfirmationGrpcRequest request)
    {
        var confirmationLink = await _confirmationLinkRepository.FindByConfirmationCodeAsync(request.ConfirmationCode);

        if (confirmationLink == null)
            return new RegisterConfirmationGrpcResponse { Status = GrpcResponseStatus.NotFound };

        var user = await _userRepository.FindByIdAsync(confirmationLink.UserId);

        if (user == null)
            return new RegisterConfirmationGrpcResponse { Status = GrpcResponseStatus.NotFound };

        await _userRepository.UpdateStatusAsync(user.Id, UserStatus.Confirmed);
        await _confirmationLinkRepository.DeleteByIdAsync(confirmationLink.Id);

        return new RegisterConfirmationGrpcResponse
        {
            Status = GrpcResponseStatus.Ok
        };
    }
}
