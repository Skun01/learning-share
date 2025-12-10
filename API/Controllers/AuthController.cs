using Application.Common;
using Application.DTOs.Auth;
using Application.IServices;
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
}
