using ImageService.Application.DTOs;
using ImageService.Application.Interfaces;
using ImageService.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageService.Application.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _repo;
    private readonly IStorageService _storage;

    public FileService(IFileRepository repo, IStorageService storage)
    {
        _repo = repo;
        _storage = storage;
    }

    public async Task<IEnumerable<FileDto>> ListAsync(Guid userId)
    {
        var rows = await _repo.ListAsync(userId);

        return rows.Select(r =>
        {
            var size = r.Status == "done" && r.TargetSize.HasValue ? 
            $"{r.TargetSize} KB": $"{r.OriSize:N1} KB";   // �d 1 ��p��

            return new FileDto(
                r.Id, 
                r.FileName, 
                size, 
                r.UploadDt, 
                r.Status ?? "processing",
                r.ThumbPath);
        });
    }

    public async Task<UploadResponse> UploadAsync(Guid userId, IFormFile file)
    {
        if (file.Length == 0)
            return new(false, "Empty file");

        if (file.Length > 5 * 1024 * 1024) 
            return new(false, "File exceeds 5 MB");

        // 1. �إ� FILES �����]�t�ɶ��W�^
        var ext = Path.GetExtension(file.FileName);  // e.g. ".png"
        var ts = DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var baseName = Path.GetFileNameWithoutExtension(file.FileName);
        var dbFileName = $"{baseName}_{ts}{ext}";

        var rec = new FileRecord
        {
            FileName = dbFileName,
            UserId = userId,
            OriSize = Math.Round((decimal)file.Length / 1024, 1),          // KB�A1 ��p��
            Status = "processing",
            Format = ext.TrimStart('.'),
            TargetSize = 500
        };

        await _repo.AddAsync(rec);  // SaveChanges ��i�� rec.Id

        // 2. �H�uID + ���ɦW�v�@�� GCS ����W
        var objectName = $"{rec.Id}{ext}";

        // 3. �W�Ǧ� Storage�]GCS �� Fake�^
        await _storage.UploadAsync(objectName, file.OpenReadStream(), file.ContentType);

        // ���� FILES ���O�� Status=processing�A�Y�ϧ�����ѥt�@�A�ȧ�s
        return new(true, "Uploaded");
    }
}
