namespace Service.Identity.Services;

public class HashService
{
	public string Hash(string input)
	{
		return BCrypt.Net.BCrypt.HashPassword(input, 12);
	}

	public bool VerifyHash(string input, string hashed)
	{
		return BCrypt.Net.BCrypt.Verify(input, hashed);
	}
}