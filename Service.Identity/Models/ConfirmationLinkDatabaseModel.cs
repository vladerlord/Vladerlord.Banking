namespace Service.Identity.Models;

public class ConfirmationLinkDatabaseModel
{
    public Guid Id { get; init; }
    public ConfirmationLinkType Type { get; init; }
    public string ConfirmationCode { get; init; }
    public Guid UserId { get; init; }

    public ConfirmationLinkDatabaseModel()
    {
        ConfirmationCode = string.Empty;
    }

    public ConfirmationLinkDatabaseModel(Guid id, ConfirmationLinkType type, string confirmationCode, Guid userId)
    {
        Id = id;
        Type = type;
        ConfirmationCode = confirmationCode;
        UserId = userId;
    }
}

public enum ConfirmationLinkType
{
    RegistrationConfirmation,
    PasswordReset
}
