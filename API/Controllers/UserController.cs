using Application.Common;
using Application.DTOs.User;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UserController : BaseController
{
    private readonly IUserService _service;
    public UserController(IUserService service)
    {
        _service = service;
    }

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("info")]
    [Authorize]
    public async Task<ApiResponse<bool>> UpdateInfo([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();

        var result = await HandleException(_service.UpdateProfileAsync(userId, request));

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
        var userId = GetCurrentUserId();

        var result = await HandleException(_service.GetProfileAsync(userId));

        return result;
    }

    /// <summary>
    /// Thay đổi avatar tài khoản người dùng
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost("avatar")]
    [Authorize]
    public async Task<ApiResponse<string>> UploadAvatar(IFormFile file)
    {
        var userId = GetCurrentUserId();

        var result = await HandleException(_service.UploadAvatarAsync(userId, file));

        return result;
    }

    /// <summary>
    /// Thay đổi mật khẩu tài khoản người dùng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("password")]
    [Authorize]
    public async Task<ApiResponse<bool>> ChangeUserPassword([FromBody] ChangePasswordRequest request)
    {
        var result = await HandleException(_service.ChangeUserPasswordAsync(GetCurrentUserId(), request));

        return result;
    }
}
