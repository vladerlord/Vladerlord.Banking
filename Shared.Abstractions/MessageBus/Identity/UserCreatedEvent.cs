namespace Shared.Abstractions.MessageBus.Identity;

public class UserCreatedEvent
{
    public Guid Id { get; set; }
    public string Email { get; init; } = null!;
    public string ConfirmationLink { get; init; } = null!;
}
