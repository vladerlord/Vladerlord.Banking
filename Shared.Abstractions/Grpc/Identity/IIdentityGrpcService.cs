using System.ServiceModel;
using Shared.Abstractions.Grpc.Identity.Contracts;

namespace Shared.Abstractions.Grpc.Identity;

[ServiceContract(Name = "IdentityGrpcService")]
public interface IIdentityGrpcService
{
    [OperationContract(Name = "LoginAsync")]
    Task<LoginGrpcResponse> LoginAsync(LoginGrpcRequest request);

    [OperationContract(Name = "RegisterAsync")]
    Task<RegisterUserGrpcResponse> RegisterAsync(RegisterUserGrpcRequest request);
    
    [OperationContract(Name = "VerifyTokenAsync")]
    Task<VerifyTokenGrpcResponse> VerifyTokenAsync(VerifyTokenGrpcRequest request);
}
