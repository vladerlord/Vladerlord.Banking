namespace Chassis.Gateway.ApiResponse;

public class HandleExceptionOptions
{
    public string ApiVersion { get; set; }

    public HandleExceptionOptions()
    {
        ApiVersion = "1.0.0";
    }
}
