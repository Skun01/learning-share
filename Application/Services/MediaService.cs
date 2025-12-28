using Application.DTOs.Media;
using Application.IRepositories;
using Application.IServices;
using Application.IServices.IInternal;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class MediaService : IMediaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;

    public MediaService(IUnitOfWork unitOfWork, IStorageService storageService)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task<MediaUploadResponse> UploadImageAsync(int userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ApplicationException(MessageConstant.FileUploadMessage.INVALID_EXTENSION);

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!FileUploadConstant.ALLOW_IMAGE_EXTENSIONS.Contains(extension))
            throw new ApplicationException(MessageConstant.FileUploadMessage.INVALID_EXTENSION);

        if (file.Length > FileUploadConstant.MAX_STORAGE)
            throw new ApplicationException(MessageConstant.FileUploadMessage.STORAGE_EXCEED);

        var relativePath = await _storageService.SaveFileAsync(file, FileUploadConstant.IMAGE_FOLDER);

        var mediaFile = new MediaFile
        {
            FileName = file.FileName,
            FilePath = relativePath,
            FileType = "image",
            FileSize = file.Length,
            UploadedByUserId = userId
        };

        await _unitOfWork.MediaFiles.AddAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();

        return new MediaUploadResponse
        {
            Id = mediaFile.Id,
            Url = "/" + relativePath,
            Type = "image",
            FileName = file.FileName,
            FileSize = file.Length
        };
    }

    public async Task<MediaUploadResponse> UploadAudioAsync(int userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ApplicationException(MessageConstant.FileUploadMessage.INVALID_EXTENSION);

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!FileUploadConstant.ALLOW_AUDIO_EXTENSIONS.Contains(extension))
            throw new ApplicationException(MessageConstant.FileUploadMessage.INVALID_EXTENSION);

        if (file.Length > FileUploadConstant.MAX_STORAGE)
            throw new ApplicationException(MessageConstant.FileUploadMessage.STORAGE_EXCEED);

        var relativePath = await _storageService.SaveFileAsync(file, FileUploadConstant.AUDIO_FOLDER);

        var mediaFile = new MediaFile
        {
            FileName = file.FileName,
            FilePath = relativePath,
            FileType = "audio",
            FileSize = file.Length,
            UploadedByUserId = userId
        };

        await _unitOfWork.MediaFiles.AddAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();

        return new MediaUploadResponse
        {
            Id = mediaFile.Id,
            Url = "/" + relativePath,
            Type = "audio",
            FileName = file.FileName,
            FileSize = file.Length
        };
    }

    public async Task<bool> DeleteMediaAsync(int userId, int mediaId)
    {
        var mediaFile = await _unitOfWork.MediaFiles.GetByIdAsync(mediaId);
        if (mediaFile == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        if (mediaFile.UploadedByUserId != userId)
            throw new ApplicationException(MessageConstant.CommonMessage.INVALID);

        await _storageService.DeleteFileAsync(mediaFile.FilePath);
        _unitOfWork.MediaFiles.DeleteAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
