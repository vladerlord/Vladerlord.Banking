using System.Runtime.Serialization;

namespace Shared.Grpc.PersonalData.Models;

[DataContract]
public class PersonalDataGrpcModel
{
    public PersonalDataGrpcModel()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Country = string.Empty;
        City = string.Empty;
    }

    [DataMember(Order = 1)] public Guid Id { get; set; }
    [DataMember(Order = 2)] public Guid UserId { get; set; }
    [DataMember(Order = 3)] public string FirstName { get; set; }
    [DataMember(Order = 4)] public string LastName { get; set; }
    [DataMember(Order = 5)] public string Country { get; set; }
    [DataMember(Order = 6)] public string City { get; set; }
}

[DataContract]
public class PersonalDataCreateGrpcModel
{
    public PersonalDataCreateGrpcModel()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Country = string.Empty;
        City = string.Empty;
    }

    [DataMember(Order = 1)] public Guid UserId { get; set; }
    [DataMember(Order = 2)] public string FirstName { get; set; }
    [DataMember(Order = 3)] public string LastName { get; set; }
    [DataMember(Order = 4)] public string Country { get; set; }
    [DataMember(Order = 5)] public string City { get; set; }
}
