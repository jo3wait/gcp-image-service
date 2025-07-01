using ImageService.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageService.Application.Interfaces;

public interface IFileService
{
    Task<IEnumerable<FileDto>> ListAsync(Guid userId);
    Task<UploadResponse> UploadAsync(Guid userId, IFormFile file);
}
