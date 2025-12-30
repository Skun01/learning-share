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
    private readonly JwtSettings _jwtSettings;
    
    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IEmailSenderService emailService,
        IOptions<AppSettings> appSettings, IOptions<JwtSettings> jwtSettings, IEmailTemplateService emailTemplateService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _emailService = emailService;
        _appSettings = appSettings.Value;
        _jwtSettings = jwtSettings.Value;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<AuthDTO> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if(user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new ApplicationException(MessageConstant.AuthMessage.INVALID_LOGIN);

        return await CreateAuthDTOAsync(user);
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

        return await CreateAuthDTOAsync(newUser);
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

    public async Task<AuthDTO> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshTokenEntity = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

        if (refreshTokenEntity == null)
            throw new ApplicationException(MessageConstant.AuthMessage.INVALID_REFRESH_TOKEN);

        if (refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            throw new ApplicationException(MessageConstant.AuthMessage.REFRESH_TOKEN_EXPIRED);

        // Revoke old token
        refreshTokenEntity.IsRevoked = true;
        refreshTokenEntity.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.UpdateAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        // Create new tokens
        return await CreateAuthDTOAsync(refreshTokenEntity.User);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var refreshTokenEntity = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

        if (refreshTokenEntity == null)
            throw new ApplicationException(MessageConstant.AuthMessage.INVALID_REFRESH_TOKEN);

        refreshTokenEntity.IsRevoked = true;
        refreshTokenEntity.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.UpdateAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private async Task<AuthDTO> CreateAuthDTOAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token to database
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDays),
            UserId = user.Id
        };
        
        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new AuthDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user.ToDTO()
        };
    }
}
