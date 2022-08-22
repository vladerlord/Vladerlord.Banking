using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class ListAllUnapprovedPersonalDataGrpcRequest
{
}

[DataContract]
public class ListAllUnapprovedPersonalDataGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; set; }
    [DataMember(Order = 2)] public List<PersonalDataGrpcModel> PersonalDataList { get; set; }

    public ListAllUnapprovedPersonalDataGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
        PersonalDataList = new List<PersonalDataGrpcModel>();
    }
}
