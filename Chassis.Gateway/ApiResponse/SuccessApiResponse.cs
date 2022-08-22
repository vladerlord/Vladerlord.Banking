using System.Net;
using System.Runtime.Serialization;

namespace Chassis.Gateway.ApiResponse;

[DataContract]
public class SuccessApiResponse<T> : ApiResponse
{
    [DataMember(Order = 4)] public T? Result { get; init; }

    public SuccessApiResponse(T? result, string apiVersion, int statusCode = (int)HttpStatusCode.OK)
        : base(false, statusCode, apiVersion)
    {
        Result = result;
    }
}
