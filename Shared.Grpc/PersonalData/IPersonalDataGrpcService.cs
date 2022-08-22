using System.ServiceModel;
using Shared.Grpc.PersonalData.Contracts;

namespace Shared.Grpc.PersonalData;

[ServiceContract(Name = "IPersonalDataGrpcService")]
public interface IPersonalDataGrpcService
{
    [OperationContract(Name = "ApplyPersonalDataAsync")]
    Task<ApplyPersonalDataGrpcResponse> ApplyPersonalDataAsync(ApplyPersonalDataGrpcRequest request);

    [OperationContract(Name = "ListAllUnapprovedPersonalDataAsync")]
    Task<ListAllUnapprovedPersonalDataGrpcResponse> ListAllUnapprovedPersonalDataAsync();

    [OperationContract(Name = "GetByIdAsync")]
    Task<GetPersonalDataByIdGrpcResponse> GetByIdAsync(GetPersonalDataByIdGrpcRequest request);

    [OperationContract(Name = "ApprovePersonalDataAsync")]
    Task<ApprovePersonalDataGrpcResponse> ApprovePersonalDataAsync(ApprovePersonalDataGrpcRequest request);

    [OperationContract(Name = "DeclinePersonalDataAsync")]
    Task<DeclinePersonalDataGrpcResponse> DeclinePersonalDataAsync(DeclinePersonalDataGrpcRequest request);
}
