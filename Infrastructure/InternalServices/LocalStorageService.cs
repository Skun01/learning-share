using Application.IServices.IInternal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.InternalServices;

public class LocalStorageService : IStorageService
{
    private readonly IWebHostEnvironment _env;
    public LocalStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folderName)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"uploads/{folderName}/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return Task.CompletedTask;

        var normalizedPath = relativePath.Replace('/', Path.DirectorySeparatorChar);

        var physicalPath = Path.Combine(_env.WebRootPath, normalizedPath);

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }

        return Task.CompletedTask;
    }
}
