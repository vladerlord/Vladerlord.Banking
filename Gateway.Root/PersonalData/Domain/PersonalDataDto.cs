using System.Runtime.Serialization;
using Shared.Abstractions.Grpc.PersonalData.Models;

namespace Gateway.Root.PersonalData.Domain;

[DataContract]
public class PersonalDataDto
{
	[DataMember] public Guid? Id { get; }
	[DataMember] public Guid UserId { get; }
	[DataMember] public string FirstName { get; }
	[DataMember] public string LastName { get; }
	[DataMember] public string Country { get; }
	[DataMember] public string City { get; }

	public PersonalDataDto(Guid? id, Guid userId, string firstName, string lastName, string country, string city)
	{
		Id = id;
		UserId = userId;
		FirstName = firstName;
		LastName = lastName;
		Country = country;
		City = city;
	}
}

public static class PersonalDataDtoExtensions
{
	public static PersonalDataDto ToDto(this PersonalDataGrpcModel grpcModel)
	{
		return new PersonalDataDto(grpcModel.Id, grpcModel.UserId, grpcModel.FirstName, grpcModel.LastName,
			grpcModel.Country, grpcModel.City);
	}
}
