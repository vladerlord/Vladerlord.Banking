using System.Runtime.Serialization;

namespace Shared.Abstractions.Grpc.PersonalData.Models;

[DataContract]
public class KycScanGrpcModel
{
	[DataMember(Order = 1)] public Guid Id { get; init; }
	[DataMember(Order = 2)] public string FileName { get; set; } = null!;
	[DataMember(Order = 3)] public string FileExtension { get; set; } = null!;
	[DataMember(Order = 4)] public byte[] Content { get; set; } = null!;
}