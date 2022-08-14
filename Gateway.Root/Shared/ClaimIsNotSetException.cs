namespace Gateway.Root.Shared;

public class ClaimIsNotSetException : Exception
{
	public ClaimIsNotSetException(string claimType) : base($"Claim's {claimType} values is missing")
	{
	}
}