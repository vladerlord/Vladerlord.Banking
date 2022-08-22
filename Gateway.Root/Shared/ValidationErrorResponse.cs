using System.Runtime.Serialization;

namespace Gateway.Root.Shared;

[DataContract]
public class ValidationErrorResponse
{
    [DataMember] public Dictionary<string, List<string>> Errors { get; }

    public ValidationErrorResponse(Dictionary<string, List<string>> errors)
    {
        Errors = errors;
    }
}
