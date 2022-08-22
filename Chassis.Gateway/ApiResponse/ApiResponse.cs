using System.Runtime.Serialization;

namespace Chassis.Gateway.ApiResponse;

[DataContract]
public abstract class ApiResponse
{
    [DataMember(Order = 1)] public string ApiVersion { get; }
    [DataMember(Order = 2)] public bool IsError { get; }
    [DataMember(Order = 3)] public int StatusCode { get; }

    protected ApiResponse(bool isError, int statusCode, string apiVersion)
    {
        IsError = isError;
        StatusCode = statusCode;
        ApiVersion = apiVersion;
    }
}
