using System;

namespace ImageService.Application.DTOs;

public record FileDto(
    Guid Id,
    string FileName,
    string OriSizeKb,
    string CompSizeKb,
    DateTime UploadDate,
    string Status,
    string? ThumbUrl);
