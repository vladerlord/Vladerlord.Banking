namespace Service.IdentityNotifier;

public interface IMailSender
{
    public Task SendLetter(string to, string subject, string body);
}
