using System.ServiceModel;
using Shared.Grpc.Identity.Contracts;

namespace Shared.Grpc.Identity;

[ServiceContract(Name = "IdentityGrpcService")]
public interface IIdentityGrpcService
{
    [OperationContract(Name = "LoginAsync")]
    Task<LoginGrpcResponse> LoginAsync(LoginGrpcRequest request);

    [OperationContract(Name = "RegisterAsync")]
    Task<RegisterUserGrpcResponse> RegisterAsync(RegisterUserGrpcRequest request);

    [OperationContract(Name = "VerifyTokenAsync")]
    Task<VerifyTokenGrpcResponse> VerifyTokenAsync(VerifyTokenGrpcRequest request);

    [OperationContract(Name = "RegisterConfirmationAsync")]
    Task<RegisterConfirmationGrpcResponse> RegisterConfirmationAsync(RegisterConfirmationGrpcRequest request);
}
