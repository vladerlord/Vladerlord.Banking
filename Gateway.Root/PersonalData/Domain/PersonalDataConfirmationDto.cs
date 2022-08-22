using Shared.Grpc.PersonalData.Models;

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

    public PersonalDataCreateGrpcModel ToPersonalDataCreateGrpcModel()
    {
        return new PersonalDataCreateGrpcModel
        {
            UserId = UserId,
            FirstName = FirstName,
            LastName = LastName,
            Country = Country,
            City = City
        };
    }

    public async Task<List<KycScanCreateGrpcModel>> ToKycScanCreateGrpcModels()
    {
        var scans = new List<KycScanCreateGrpcModel>();

        foreach (var scan in KycScans)
        {
            await using var ms = new MemoryStream();
            await scan.CopyToAsync(ms);

            scans.Add(new KycScanCreateGrpcModel
            {
                FileName = Path.GetFileNameWithoutExtension(scan.FileName),
                FileExtension = Path.GetExtension(scan.FileName),
                Content = ms.ToArray(),
                ContentType = scan.ContentType
            });
        }

        return scans;
    }
}
