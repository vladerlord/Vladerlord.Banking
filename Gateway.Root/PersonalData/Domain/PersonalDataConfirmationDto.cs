using Shared.Abstractions.Grpc.PersonalData.Models;

namespace Gateway.Root.PersonalData.Domain;

public class PersonalDataConfirmationDto
{
	public Guid UserId { get; }
	public string FirstName { get; }
	public string LastName { get; }
	public string Country { get; }
	public string City { get; }
	public IFormFileCollection KycScans { get; }

	public PersonalDataConfirmationDto(
		Guid userId,
		string firstName,
		string lastName,
		string country,
		string city,
		IFormFileCollection kycScans)
	{
		UserId = userId;
		FirstName = firstName;
		LastName = lastName;
		Country = country;
		City = city;
		KycScans = kycScans;
	}

	public PersonalDataGrpcModel ToPersonalDataGrpcModel()
	{
		return new PersonalDataGrpcModel
		{
			UserId = UserId,
			FirstName = FirstName,
			LastName = LastName,
			Country = Country,
			City = City
		};
	}

	public async Task<List<KycScanGrpcModel>> ToKycScanGrpcModels()
	{
		var scans = new List<KycScanGrpcModel>();

		foreach (var scan in KycScans)
		{
			await using var ms = new MemoryStream();
			await scan.CopyToAsync(ms);

			scans.Add(new KycScanGrpcModel
			{
				FileName = Path.GetFileNameWithoutExtension(scan.FileName),
				FileExtension = Path.GetExtension(scan.FileName),
				Content = ms.ToArray()
			});
		}

		return scans;
	}
}