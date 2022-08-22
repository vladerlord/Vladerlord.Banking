using Gateway.Root.Identity.Domain;
using Gateway.Root.Shared;
using Shared.Grpc.Identity;
using Shared.Grpc.Identity.Contracts;

namespace Gateway.Root.Identity.Application;

public class UserService
{
    private readonly IIdentityGrpcService _identityGrpcService;

    public UserService(IIdentityGrpcService identityGrpcService)
    {
        _identityGrpcService = identityGrpcService;
    }

    public async Task<GrpcAppResponse<LoginDto>> LoginAsync(string email, string password)
    {
        var grpRequest = new LoginGrpcRequest
        {
            Email = email,
            Password = password
        };
        var response = await _identityGrpcService.LoginAsync(grpRequest);

        return new GrpcAppResponse<LoginDto>
        {
            GrpcStatus = response.GrpcResponse,
            Content = response.ToDto()
        };
    }

    public async Task<GrpcAppResponse<UserDto?>> RegisterAsync(string confirmationCode, string confirmationUrl,
        string email, string password)
    {
        var grpcRequest = new RegisterUserGrpcRequest
        {
            Email = email,
            Password = password,
            ConfirmationCode = confirmationCode,
            RegisterConfirmationUrl = confirmationUrl
        };

        var response = await _identityGrpcService.RegisterAsync(grpcRequest);

        return new GrpcAppResponse<UserDto?>
        {
            GrpcStatus = response.GrpcResponse,
            Content = response.UserModel?.ToDto()
        };
    }
}
