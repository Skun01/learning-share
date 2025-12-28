using Application.DTOs.Media;
using Microsoft.AspNetCore.Http;

namespace Application.IServices;

public interface IMediaService
{
    Task<MediaUploadResponse> UploadImageAsync(int userId, IFormFile file);
    Task<MediaUploadResponse> UploadAudioAsync(int userId, IFormFile file);
    Task<bool> DeleteMediaAsync(int userId, int mediaId);
}
