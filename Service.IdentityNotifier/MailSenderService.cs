using System.Net;
using System.Net.Mail;

namespace Service.IdentityNotifier;

public class MailSenderService : IMailSender
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _from;
    private readonly NetworkCredential _smtpCredentials;

    public MailSenderService(string host, int port, string username, string password, string from)
    {
        _host = host;
        _port = port;
        _from = from;
        _smtpCredentials = new NetworkCredential(username, password);
    }

    public async Task SendLetter(string to, string subject, string body)
    {
        using var smtp = new SmtpClient();

        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.UseDefaultCredentials = false;
        smtp.EnableSsl = true;
        smtp.Host = _host;
        smtp.Port = _port;
        smtp.Credentials = _smtpCredentials;

        var message = new MailMessage
        {
            From = new MailAddress(_from),
            Subject = subject,
            IsBodyHtml = true,
            Body = body,
        };
        message.To.Add(to);

        await smtp.SendMailAsync(message);
    }
}