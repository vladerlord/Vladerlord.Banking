namespace Chassis;

public static class EnvironmentUtils
{
    public static string GetRequiredEnvironmentVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);

        if (value == null)
            throw new EnvironmentValueNotSetException(name);

        return value;
    }
}
