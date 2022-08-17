using System.ServiceModel;
using Shared.Grpc.PersonalData.Contracts;

namespace Shared.Grpc.PersonalData;

[ServiceContract(Name = "IKycScanGrpcService")]
public interface IKycScanGrpcService
{
	[OperationContract(Name = "FindByIdAsync")]
	Task<FindKycScanByIdGrpcResponse> FindByIdAsync(FindKycScanByIdGrpcRequest request);
}
