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
    private readonly TokenAuthService _tokenAuthService;

    public IdentityGrpcService(IUserRepository userRepository, TokenAuthService tokenAuthService)
    {
        _userRepository = userRepository;
        _tokenAuthService = tokenAuthService;
    }

    public async Task<LoginGrpcResponse> LoginAsync(LoginGrpcRequest request)
    {
        var user = await _userRepository.FindByEmail(request.Email);

        if (user == null)
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
            var iv = EncryptionService.GenerateIV();

            user = await _userRepository.CreateAsync(request.ToDatabaseModel(hashedPassword, iv));
        }
        catch (PostgresException e)
        {
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
}