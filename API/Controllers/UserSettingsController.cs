using Application.Common;
using Application.DTOs.UserSettings;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/settings")]
[Authorize]
public class UserSettingsController : BaseController
{
    private readonly IUserSettingsService _service;
    public UserSettingsController(IUserSettingsService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy thông tin cài đặt tài khoản người dùng
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResponse<UserSettingsDTO>> GetSettings()
    {
        var result = await HandleException(_service.GetSettingsAsync(GetCurrentUserId()));

        return result;
    }

    /// <summary>
    /// thay đổi chế độ ghost-mode
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("ghost-mode")]
    public async Task<ApiResponse<bool>> UpdateGoshMode(UpdateGoshModeRequest request)
    {
        var result = await HandleException(_service.UpdateGhostModeAsync(GetCurrentUserId(), request));

        return result;
    }

    /// <summary>
    /// Thay đổi mục tiêu học hàng ngày
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("daily-goal")]
    public async Task<ApiResponse<bool>> UpdateDailyGoal([FromBody] UpdateDailyGoalRequest request)
    {
        var result = await HandleException(_service.UpdateDailyGoalAsync(GetCurrentUserId(), request));

        return result;
    }

    /// <summary>
    /// Thay đổi ngôn ngữ tài khoản
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("language")]
    public async Task<ApiResponse<bool>> UpdateLanguage([FromBody] UpdateLanguageRequest request)
    {
        var result = await HandleException(_service.UpdateUiLanguageAsync(GetCurrentUserId(), request));

        return result;
    }
}
