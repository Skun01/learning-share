using System.Security.Claims;
using Application.Common;
using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Đăng nhập tài khoản
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ApiResponse<AuthDTO>> Login(LoginRequest request)
    {
        var result = await HandleException(_service.LoginAsync(request));

        return result;
    }

    /// <summary>
    /// Đăng ký tài khoản
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ApiResponse<AuthDTO>> Register(RegisterRequest request)
    {
        var result = await HandleException(_service.RegisterAsync(request));

        return result;
    }

    /// <summary>
    /// Lấy thông tin tài khoản người dùng đang đăng nhập
    /// </summary>
    /// <returns></returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<ApiResponse<UserProfileDTO>> GetMe()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await HandleException(_service.GetProfileAsync(int.Parse(userIdString!)));

        return result;
    }
}
