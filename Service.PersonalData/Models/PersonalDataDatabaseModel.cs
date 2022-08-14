using Shared.Abstractions.Grpc.PersonalData.Contracts;

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

	public PersonalDataDatabaseModel()
	{
		FirstName = string.Empty;
		LastName = string.Empty;
		Country = string.Empty;
		City = string.Empty;
		Iv = string.Empty;
	}

	public PersonalDataDatabaseModel(Guid id, Guid userId, string firstName, string lastName, string country,
		string city, string iv)
	{
		Id = id;
		UserId = userId;
		FirstName = firstName;
		LastName = lastName;
		Country = country;
		City = city;
		Iv = iv;
	}
}

public static class PersonalDataGrpcRequestExtensions
{
	public static PersonalDataDatabaseModel ToPersonalDataDatabaseModel(this ApplyPersonalDataGrpcRequest request,
		string iv)
	{
		return new PersonalDataDatabaseModel(
			Guid.NewGuid(),
			request.PersonalData.UserId,
			request.PersonalData.FirstName,
			request.PersonalData.LastName,
			request.PersonalData.Country,
			request.PersonalData.City,
			iv
		);
	}
}