using System.Runtime.Serialization;

namespace Shared.Grpc.Identity.Models;

[DataContract]
public class UserGrpcModel
{
    [DataMember(Order = 1)] public Guid Id;
    [DataMember(Order = 2)] public string Email;

    public UserGrpcModel(Guid id, string email)
    {
        Id = id;
        Email = email;
    }
}