using System.Runtime.Serialization;
using Gateway.Root.Identity.Domain;
using Gateway.Root.Shared;

namespace Gateway.Root.Identity.Presentation.Models;

[DataContract]
public class LoginUserHttpRequest
{
    [DataMember] public string Email { get; set; }
    [DataMember] public string Password { get; set; }

    public LoginUserHttpRequest()
    {
        Email = string.Empty;
        Password = string.Empty;
    }
}

[DataContract]
public class LoginUserHttpResponse : GatewayHttpResponse
{
    [DataMember] public string? Jwt { get; init; }
    [DataMember] public UserDto? User { get; init; }
}
