using System.ServiceModel;
using Shared.Abstractions.Grpc.PersonalData.Contracts;

namespace Shared.Abstractions.Grpc.PersonalData;

[ServiceContract(Name = "IPersonalDataGrpcService")]
public interface IPersonalDataGrpcService
{
	[OperationContract(Name = "ApplyPersonalDataAsync")]
	Task<ApplyPersonalDataGrpcResponse> ApplyPersonalDataAsync(ApplyPersonalDataGrpcRequest request);
}