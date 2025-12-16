namespace Application.Settings;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    public string SmtpServer { set; get; } = string.Empty;
    public int Port { set; get; }
    public string FromAddress { set; get; } = string.Empty;
    public string Password { set; get; } = string.Empty;
}
