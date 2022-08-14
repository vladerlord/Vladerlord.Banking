namespace Chassis;

public class EnvironmentValueNotSetException : Exception
{
	public EnvironmentValueNotSetException(string name) : base($"Env {name} is not set")
	{
	}
}