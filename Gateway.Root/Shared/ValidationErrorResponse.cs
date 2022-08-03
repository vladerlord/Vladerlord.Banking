namespace Gateway.Root.Shared;

public class ValidationErrorResponse
{
    public Dictionary<string, List<string>> Errors { get; }

    public ValidationErrorResponse(Dictionary<string, List<string>> errors)
    {
        Errors = errors;
    }
}
