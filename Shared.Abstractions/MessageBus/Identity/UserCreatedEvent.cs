namespace Shared.Abstractions.MessageBus.Identity;

public class UserCreatedEvent
{
    public Guid Id { get; init; }
    public string Email { get; init; }
}
