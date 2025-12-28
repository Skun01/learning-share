using Application.Common;
using Application.DTOs.Media;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/media")]
[Authorize]
public class MediaController : BaseController
{
    private readonly IMediaService _service;

    public MediaController(IMediaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Upload hình ảnh
    /// </summary>
    [HttpPost("image")]
    public async Task<ApiResponse<MediaUploadResponse>> UploadImageAsync(IFormFile file)
    {
        var result = await HandleException(_service.UploadImageAsync(GetCurrentUserId(), file));
        return result;
    }

    /// <summary>
    /// Upload file âm thanh
    /// </summary>
    [HttpPost("audio")]
    public async Task<ApiResponse<MediaUploadResponse>> UploadAudioAsync(IFormFile file)
    {
        var result = await HandleException(_service.UploadAudioAsync(GetCurrentUserId(), file));
        return result;
    }

    /// <summary>
    /// Xóa file media
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteMediaAsync(int id)
    {
        var result = await HandleException(_service.DeleteMediaAsync(GetCurrentUserId(), id));
        return result;
    }
}
