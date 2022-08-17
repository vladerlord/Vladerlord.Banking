using Shared.Grpc.PersonalData.Contracts;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Models;

public class PersonalDataDatabaseModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Country { get; set; }
	public string City { get; set; }
	public string Iv { get; set; }
	public PersonalDataStatus Status { get; set; }

	public PersonalDataDatabaseModel()
	{
		FirstName = string.Empty;
		LastName = string.Empty;
		Country = string.Empty;
		City = string.Empty;
		Iv = string.Empty;
	}

	public PersonalDataDatabaseModel(Guid id, Guid userId, string firstName, string lastName, string country,
		string city, string iv, PersonalDataStatus status)
	{
		Id = id;
		UserId = userId;
		FirstName = firstName;
		LastName = lastName;
		Country = country;
		City = city;
		Iv = iv;
		Status = status;
	}

	public PersonalDataGrpcModel ToGrpcModel()
	{
		return new PersonalDataGrpcModel
		{
			Id = Id,
			UserId = UserId,
			FirstName = FirstName,
			LastName = LastName,
			Country = Country,
			City = City
		};
	}
}

public enum PersonalDataStatus
{
	PendingApproval,
	Approved,
	Declined
}

public static class PersonalDataGrpcRequestExtensions
{
	public static PersonalDataDatabaseModel ToPersonalDataDatabaseModel(this ApplyPersonalDataGrpcRequest request,
		string iv, PersonalDataStatus status)
	{
		return new PersonalDataDatabaseModel(
			Guid.NewGuid(),
			request.PersonalData.UserId,
			request.PersonalData.FirstName,
			request.PersonalData.LastName,
			request.PersonalData.Country,
			request.PersonalData.City,
			iv,
			status
		);
	}
}