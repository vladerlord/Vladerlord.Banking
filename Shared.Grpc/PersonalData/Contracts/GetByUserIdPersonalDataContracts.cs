using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class GetByUserIdPersonalDataByIdGrpcRequest
{
    [DataMember(Order = 1)] public Guid UserId { get; init; }
}

[DataContract]
public class GetByUserIdPersonalDataByIdGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public PersonalDataGrpcModel? PersonalData { get; init; }

    public GetByUserIdPersonalDataByIdGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
