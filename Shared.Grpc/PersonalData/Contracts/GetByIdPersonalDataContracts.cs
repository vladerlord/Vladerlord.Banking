using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class GetPersonalDataByIdGrpcRequest
{
    [DataMember(Order = 1)] public Guid PersonalDataId { get; init; }
}

[DataContract]
public class GetPersonalDataByIdGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public PersonalDataGrpcModel? PersonalData { get; init; }
    [DataMember(Order = 3)] public List<Guid> KycScansIds { get; init; }

    public GetPersonalDataByIdGrpcResponse()
    {
        KycScansIds = new List<Guid>();
        GrpcResponse = new GrpcResponse();
    }
}
