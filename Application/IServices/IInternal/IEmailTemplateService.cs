namespace Application.IServices.IInternal;

public interface IEmailTemplateService
{
    string GetPasswordResetTemplate(string username, string resetUrl, int expiresMinutes);
}
