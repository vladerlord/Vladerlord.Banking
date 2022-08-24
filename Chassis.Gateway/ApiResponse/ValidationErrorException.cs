using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chassis.Gateway.ApiResponse;

public class ValidationErrorException : Exception
{
    public readonly Dictionary<string, List<string>> Errors = new();

    public ValidationErrorException()
    {
    }

    public ValidationErrorException(ModelStateDictionary modelStateDictionary)
    {
        foreach (var (key, modelState) in modelStateDictionary)
        {
            if (modelState.Errors.Count == 0)
                continue;

            if (!Errors.ContainsKey(key))
                Errors.Add(key, new List<string>());

            foreach (var modelStateError in modelState.Errors)
                Errors[key].Add(modelStateError.ErrorMessage);
        }
    }

    public void AddError(string key, string message)
    {
        if (!Errors.ContainsKey(key))
            Errors.Add(key, new List<string>());

        Errors[key].Add(message);
    }
}
