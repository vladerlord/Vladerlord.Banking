using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class FindKycScanByIdGrpcRequest
{
    [DataMember(Order = 1)] public Guid Id { get; init; }
}

[DataContract]
public class FindKycScanByIdGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public KycScanGrpcModel? KycScan { get; init; }

    public FindKycScanByIdGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}

[DataContract]
public class FindKycScansByPersonalDataIdGrpcRequest
{
    [DataMember(Order = 1)] public Guid PersonalDataId { get; init; }
}

[DataContract]
public class FindKycScansByPersonalDataIdGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public List<KycScanGrpcModel> KycScans { get; init; }

    public FindKycScansByPersonalDataIdGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
        KycScans = new List<KycScanGrpcModel>();
    }
}
