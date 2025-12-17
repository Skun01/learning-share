using Microsoft.AspNetCore.Http;

namespace Application.IServices.IInternal;

public interface IStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName);
    Task DeleteFileAsync(string relativePath);
}
