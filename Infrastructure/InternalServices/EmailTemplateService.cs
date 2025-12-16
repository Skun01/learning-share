using Application.IServices.IInternal;
using Application.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.InternalServices;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly AppSettings _appSettings;
    public EmailTemplateService(IOptions<AppSettings> settings)
    {
        _appSettings = settings.Value;
    }

    public string GetPasswordResetTemplate(string username, string resetUrl, int expiresMinutes)
    {
        return $@"
        <!DOCTYPE html>
        <html lang=""vi"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>{_appSettings.Name}: Đặt lại mật khẩu</title>
        </head>
        <body style=""margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 100vh;"">
            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; padding: 40px 20px;"">
                <tr>
                    <td align=""center"">
                        <table role=""presentation"" style=""max-width: 600px; width: 100%; background: #ffffff; border-radius: 16px; box-shadow: 0 20px 60px rgba(0,0,0,0.3); overflow: hidden;"">
                            <!-- Header -->
                            <tr>
                                <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 40px 30px; text-align: center;"">
                                    <div style=""background: rgba(255,255,255,0.2); width: 80px; height: 80px; border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center; backdrop-filter: blur(10px);"">
                                        <svg width=""40"" height=""40"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                                            <path d=""M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"" fill=""white""/>
                                        </svg>
                                    </div>
                                    <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: 700; letter-spacing: -0.5px;"">Đặt lại mật khẩu</h1>
                                </td>
                            </tr>
                            
                            <!-- Content -->
                            <tr>
                                <td style=""padding: 40px;"">
                                    <p style=""margin: 0 0 16px; color: #374151; font-size: 16px; line-height: 1.6;"">
                                        Xin chào <strong style=""color: #667eea;"">{username}</strong>,
                                    </p>
                                    <p style=""margin: 0 0 24px; color: #6b7280; font-size: 15px; line-height: 1.6;"">
                                        Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Nhấn vào nút bên dưới để tạo mật khẩu mới:
                                    </p>
                                    
                                    <!-- CTA Button -->
                                    <table role=""presentation"" style=""width: 100%; margin: 32px 0;"">
                                        <tr>
                                            <td align=""center"">
                                                <a href=""{resetUrl}"" style=""display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; padding: 16px 48px; border-radius: 12px; font-weight: 600; font-size: 16px; box-shadow: 0 10px 25px rgba(102, 126, 234, 0.4); transition: transform 0.2s;"">
                                                    Đặt lại mật khẩu
                                                </a>
                                            </td>
                                        </tr>
                                    </table>
                                    
                                    <!-- Timer Warning -->
                                    <div style=""background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%); border-left: 4px solid #f59e0b; padding: 16px 20px; border-radius: 8px; margin: 24px 0;"">
                                        <p style=""margin: 0; color: #92400e; font-size: 14px; line-height: 1.6;"">
                                            ⏰ <strong>Lưu ý:</strong> Link này sẽ hết hạn sau <strong>{expiresMinutes} phút</strong>. Vui lòng hoàn tất việc đặt lại mật khẩu trong thời gian này.
                                        </p>
                                    </div>
                                    
                                    <!-- Alternative Link -->
                                    <div style=""background: #f9fafb; border-radius: 8px; padding: 20px; margin: 24px 0;"">
                                        <p style=""margin: 0 0 8px; color: #6b7280; font-size: 13px; font-weight: 600;"">
                                            Nếu nút không hoạt động, copy link sau vào trình duyệt:
                                        </p>
                                        <p style=""margin: 0; word-break: break-all;"">
                                            <a href=""{resetUrl}"" style=""color: #667eea; font-size: 13px; text-decoration: none;"">{resetUrl}</a>
                                        </p>
                                    </div>
                                    
                                    <!-- Security Notice -->
                                    <div style=""border-top: 2px solid #e5e7eb; padding-top: 24px; margin-top: 32px;"">
                                        <p style=""margin: 0 0 12px; color: #ef4444; font-size: 14px; line-height: 1.6;"">
                                            <strong>⚠️ Bạn không yêu cầu đặt lại mật khẩu?</strong>
                                        </p>
                                        <p style=""margin: 0; color: #6b7280; font-size: 14px; line-height: 1.6;"">
                                            Vui lòng bỏ qua email này và liên hệ với chúng tôi ngay lập tức tại <a href=""mailto:{_appSettings.SupportEmail}"" style=""color: #667eea; text-decoration: none;"">{_appSettings.SupportEmail}</a> để bảo vệ tài khoản của bạn.
                                        </p>
                                    </div>
                                </td>
                            </tr>
                            
                            <!-- Footer -->
                            <tr>
                                <td style=""background: #f9fafb; padding: 32px 40px; text-align: center; border-top: 1px solid #e5e7eb;"">
                                    <p style=""margin: 0 0 8px; color: #9ca3af; font-size: 13px;"">
                                        Email này được gửi từ <strong style=""color: #667eea;"">{_appSettings.Name}</strong>
                                    </p>
                                    <p style=""margin: 0; color: #9ca3af; font-size: 12px;"">
                                        © {DateTime.Now.Year} {_appSettings.Name}. All rights reserved.
                                    </p>
                                    <p style=""margin: 16px 0 0; color: #9ca3af; font-size: 12px;"">
                                        Cần hỗ trợ? <a href=""mailto:{_appSettings.SupportEmail}"" style=""color: #667eea; text-decoration: none;"">{_appSettings.SupportEmail}</a>
                                    </p>
                                </td>
                            </tr>
                        </table>
                        
                        <!-- Bottom spacing -->
                        <div style=""height: 40px;""></div>
                    </td>
                </tr>
            </table>
        </body>
        </html>";
    }
}
