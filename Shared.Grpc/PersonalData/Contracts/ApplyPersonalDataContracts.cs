using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class ApplyPersonalDataGrpcRequest
{
    [DataMember(Order = 1)] public PersonalDataCreateGrpcModel PersonalData { get; init; }
    [DataMember(Order = 2)] public List<KycScanCreateGrpcModel> KycScans { get; init; }

    public ApplyPersonalDataGrpcRequest()
    {
        PersonalData = new PersonalDataCreateGrpcModel();
        KycScans = new List<KycScanCreateGrpcModel>();
    }
}

[DataContract]
public class ApplyPersonalDataGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public PersonalDataGrpcModel? PersonalData { get; init; }

    public ApplyPersonalDataGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
