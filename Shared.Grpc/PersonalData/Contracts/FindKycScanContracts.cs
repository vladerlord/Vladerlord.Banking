using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class FindKycScanByIdGrpcRequest
{
	[DataMember(Order = 1)] public Guid Id { get; set; }
}

[DataContract]
public class FindKycScanByIdGrpcResponse
{
	[DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; set; }
	[DataMember(Order = 2)] public KycScanGrpcModel? KycScan { get; set; }

	public FindKycScanByIdGrpcResponse()
	{
		GrpcResponse = new GrpcResponse();
	}
}

[DataContract]
public class FindKycScansByPersonalDataIdGrpcRequest
{
	[DataMember(Order = 1)] public Guid PersonalDataId { get; set; }
}

[DataContract]
public class FindKycScansByPersonalDataIdGrpcResponse
{
	[DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; set; }
	[DataMember(Order = 2)] public List<KycScanGrpcModel> KycScans { get; set; }

	public FindKycScansByPersonalDataIdGrpcResponse()
	{
		GrpcResponse = new GrpcResponse();
		KycScans = new List<KycScanGrpcModel>();
	}
}