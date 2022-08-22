namespace Gateway.Root.Identity.Application;

public class UserResetService
{
    private readonly string _domain;
    private readonly LinkGenerator _linkGenerator;

    public UserResetService(string domain, LinkGenerator linkGenerator)
    {
        _domain = domain;
        _linkGenerator = linkGenerator;
    }

    public string GenerateConfirmationCode()
    {
        return Guid.NewGuid().ToString("N");
    }

    public string BuildUserRegistrationConfirmationBaseUri(string confirmationCode)
    {
        var actionUri = _linkGenerator.GetPathByAction("RegisterConfirmation", "User", new
        {
            confirmationCode
        });

        return $"{_domain}{actionUri}";
    }
}
