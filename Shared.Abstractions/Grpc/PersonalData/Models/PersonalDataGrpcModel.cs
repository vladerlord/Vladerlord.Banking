using System.Runtime.Serialization;

namespace Shared.Abstractions.Grpc.PersonalData.Models;

[DataContract]
public class PersonalDataGrpcModel
{
	[DataMember(Order = 1)] public Guid? Id { get; set; }
	[DataMember(Order = 2)] public Guid UserId { get; set; }
	[DataMember(Order = 3)] public string FirstName { get; set; } = null!;
	[DataMember(Order = 4)] public string LastName { get; set; } = null!;
	[DataMember(Order = 5)] public string Country { get; set; } = null!;
	[DataMember(Order = 6)] public string City { get; set; } = null!;
}