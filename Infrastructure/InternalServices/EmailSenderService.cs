using System.Net;
using System.Net.Mail;
using Application.IServices.IInternal;
using Application.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.InternalServices;

public class EmailSenderService : IEmailSenderService
{
    private readonly EmailSettings _settings;
    public EmailSenderService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.FromAddress, _settings.Password),
            EnableSsl = true  
        };

        return client.SendMailAsync(
            new MailMessage(_settings.FromAddress, email, subject, htmlMessage) {IsBodyHtml = true}
        );
    }
}
