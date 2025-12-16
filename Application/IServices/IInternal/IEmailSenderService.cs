namespace Application.IServices.IInternal;

public interface IEmailSenderService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}
