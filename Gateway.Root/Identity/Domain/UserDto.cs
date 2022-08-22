using System.Runtime.Serialization;
using Shared.Grpc.Identity.Models;

namespace Gateway.Root.Identity.Domain;

[DataContract]
public class UserDto
{
    [DataMember(Order = 1)] public Guid Id { get; init; }
    [DataMember(Order = 2)] public string Email { get; init; }

    public UserDto()
    {
        Email = string.Empty;
    }
}

public static class UserDtoExtensions
{
    public static UserDto ToDto(this UserGrpcModel userGrpcModel)
    {
        return new UserDto
        {
            Id = userGrpcModel.Id,
            Email = userGrpcModel.Email
        };
    }
}
