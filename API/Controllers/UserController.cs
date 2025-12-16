using System.Security.Claims;
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
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await HandleException(_service.UpdateProfileAsync(int.Parse(userIdString!), request));

        return result;
    }
}
