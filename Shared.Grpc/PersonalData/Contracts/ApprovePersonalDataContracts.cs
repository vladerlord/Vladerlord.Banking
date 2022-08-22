using System.Runtime.Serialization;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class ApprovePersonalDataGrpcRequest
{
    [DataMember(Order = 1)] public Guid PersonalDataId { get; init; }
}

[DataContract]
public class ApprovePersonalDataGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }

    public ApprovePersonalDataGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
