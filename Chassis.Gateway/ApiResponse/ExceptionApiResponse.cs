using System.Net;
using System.Runtime.Serialization;

namespace Chassis.Gateway.ApiResponse;

[DataContract]
public class ExceptionApiResponse : ApiResponse
{
    [DataMember] public string ExceptionType { get; set; }
    [DataMember] public string ExceptionMessage { get; set; }

    public ExceptionApiResponse(string apiVersion, int statusCode = (int)HttpStatusCode.InternalServerError) : base(
        true, statusCode, apiVersion)
    {
        ExceptionType = string.Empty;
        ExceptionMessage = string.Empty;
    }
}
