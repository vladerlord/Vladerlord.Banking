using System.Net;
using System.Runtime.Serialization;

namespace Chassis.Gateway.ApiResponse;

[DataContract]
public class ValidationApiResponse : ApiResponse
{
    [DataMember] public Dictionary<string, List<string>> ValidationErrors { get; }

    public ValidationApiResponse(Dictionary<string, List<string>> errors, string apiVersion)
        : base(true, (int)HttpStatusCode.UnprocessableEntity, apiVersion)
    {
        ValidationErrors = errors;
    }
}
