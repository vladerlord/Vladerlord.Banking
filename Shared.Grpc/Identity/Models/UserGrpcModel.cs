using System.Runtime.Serialization;

namespace Shared.Grpc.Identity.Models;

[DataContract]
public class UserGrpcModel
{
    [DataMember(Order = 1)] public Guid Id { get; init; }
    [DataMember(Order = 2)] public string Email { get; init; }

    public UserGrpcModel()
    {
        Email = string.Empty;
    }
}
