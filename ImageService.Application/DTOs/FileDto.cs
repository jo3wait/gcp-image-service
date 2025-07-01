using System;

namespace ImageService.Application.DTOs;

public record FileDto(
    Guid Id,
    string FileName,
    string Size, // xx MB
    DateTime UploadDate,
    string Status,
    string? ThumbUrl);
