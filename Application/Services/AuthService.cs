using Application.DTOs.Auth;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;
using Domain.Constants;
using Domain.Entities;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
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

    private AuthDTO CreateAuthDTO(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new AuthDTO(accessToken, refreshToken, user.ToDTO());
    }
}
