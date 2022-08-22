using System.Runtime.Serialization;
using Shared.Grpc.Identity.Contracts;

namespace Gateway.Root.Identity.Domain;

[DataContract]
public class LoginDto
{
    [DataMember] public string? Jwt { get; init; }
    [DataMember] public UserDto? User { get; init; }
}

public static class LoginDtoExtensions
{
    public static LoginDto ToDto(this LoginGrpcResponse response)
    {
        return new LoginDto
        {
            Jwt = response.Jwt,
            User = response.UserModel?.ToDto()
        };
    }
}
