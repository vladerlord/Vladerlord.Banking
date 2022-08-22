using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Gateway.Root.PersonalData.Domain;

[DataContract]
public class PersonalDataDto
{
    [DataMember] public Guid Id { get; init; }
    [DataMember] public Guid UserId { get; init; }
    [DataMember] public string FirstName { get; init; }
    [DataMember] public string LastName { get; init; }
    [DataMember] public string Country { get; init; }
    [DataMember] public string City { get; init; }

    [DataMember] public List<Guid> KycScansIds { get; init; }

    public PersonalDataDto()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Country = string.Empty;
        City = string.Empty;
        KycScansIds = new List<Guid>();
    }
}

public static class PersonalDataDtoExtensions
{
    public static PersonalDataDto ToDto(this PersonalDataGrpcModel grpcModel, List<Guid>? kycScansIds = null)
    {
        return new PersonalDataDto
        {
            Id = grpcModel.Id,
            UserId = grpcModel.UserId,
            FirstName = grpcModel.FirstName,
            LastName = grpcModel.LastName,
            Country = grpcModel.Country,
            City = grpcModel.City,
            KycScansIds = kycScansIds ?? new List<Guid>()
        };
    }
}
