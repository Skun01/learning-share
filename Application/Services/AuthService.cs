using Application.DTOs.Auth;
using Application.IRepositories;
using Application.IServices;
using Application.IServices.IInternal;
using Application.Mappings;
using Application.Settings;
using Domain.Constants;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IEmailSenderService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly AppSettings _appSettings;
    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IEmailSenderService emailService,
        IOptions<AppSettings> _settings, IEmailTemplateService emailTemplateService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _emailService = emailService;
        _appSettings = _settings.Value;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<AuthDTO> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if(user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new ApplicationException(MessageConstant.AuthMessage.INVALID_LOGIN);

        return CreateAuthDTO(user);
    }
    
    public async Task<AuthDTO> RegisterAsync(RegisterRequest request)
    {
        var isEmailExist = await _unitOfWork.Users.AnyAsync(u => u.Email == request.Email);
        if(isEmailExist)
            throw new ApplicationException(MessageConstant.UserMessage.EMAIL_ALREADY_EXISTS);

        var newUser = request.ToEntityFromRegisterRequest();
        newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        var settings = new UserSettings
        {
            UserId = newUser.Id,
        };

        await _unitOfWork.UserSettings.AddAsync(settings);
        await _unitOfWork.SaveChangesAsync();

        return CreateAuthDTO(newUser);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(request.Token);

        if(user == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        if(user.PasswordResetTokenExpiry < DateTime.UtcNow)
            throw new ApplicationException(MessageConstant.AuthMessage.RESET_PASSWORD_TOKEN_EXPIRED);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SendResetPasswordEmailAsync(ForgotPasswordRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if(user != null)
        {
            user.PasswordResetToken = _tokenService.GenerateRandomToken();
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
            string resetUrl = $"{_appSettings.ResetPasswordUrl}?token={user.PasswordResetToken}";
            string emailTemplate = _emailTemplateService.GetPasswordResetTemplate(user.Username, resetUrl, 15);

            await _emailService.SendEmailAsync(
                user.Email,
                EmailSubjectConstant.RESET_PASSWORD,
                emailTemplate
            );
        }

        return true;
    }

    private AuthDTO CreateAuthDTO(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new AuthDTO(accessToken, refreshToken, user.ToDTO());
    }
}
