namespace Service.IdentityNotifier.RazorTemplates;

public class UserConfirmation : Layout
{
    public string ConfirmationLink;

    public UserConfirmation(string title, string confirmationLink) : base(title)
    {
        ConfirmationLink = confirmationLink;
    }
}
