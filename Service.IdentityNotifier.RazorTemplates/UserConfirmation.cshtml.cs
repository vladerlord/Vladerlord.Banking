namespace Service.IdentityNotifier.RazorTemplates;

public class UserConfirmation : Layout
{
    public string ConfirmationLink;

    public UserConfirmation(string title) : base(title)
    {
    }
}