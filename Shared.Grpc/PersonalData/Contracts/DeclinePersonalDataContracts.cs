using System.Runtime.Serialization;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class DeclinePersonalDataGrpcRequest
{
    [DataMember(Order = 1)] public Guid PersonalDataId { get; init; }
}

[DataContract]
public class DeclinePersonalDataGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }

    public DeclinePersonalDataGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
