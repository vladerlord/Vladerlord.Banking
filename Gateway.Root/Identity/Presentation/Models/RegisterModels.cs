using System.Runtime.Serialization;
using Gateway.Root.Identity.Domain;
using Gateway.Root.Shared;

namespace Gateway.Root.Identity.Presentation.Models;

[DataContract]
public class RegisterUserHttpRequest
{
    [DataMember] public string Email { get; init; }
    [DataMember] public string Password { get; init; }

    public RegisterUserHttpRequest()
    {
        Email = string.Empty;
        Password = string.Empty;
    }
}

[DataContract]
public class RegisterUserHttpResponse : GatewayHttpResponse
{
    [DataMember] public UserDto? User { get; init; }
}
