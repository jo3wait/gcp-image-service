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
            $"{r.TargetSize} KB": $"{r.OriSize:N1} KB";   // 留 1 位小數

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

        // 1. 建立 FILES 紀錄（含時間戳）
        var ext = Path.GetExtension(file.FileName);  // e.g. ".png"
        var ts = DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var baseName = Path.GetFileNameWithoutExtension(file.FileName);
        var dbFileName = $"{baseName}_{ts}{ext}";

        var rec = new FileRecord
        {
            FileName = dbFileName,
            UserId = userId,
            OriSize = Math.Round((decimal)file.Length / 1024, 1),          // KB，1 位小數
            Status = "processing",
            Format = ext.TrimStart('.'),
            TargetSize = 500
        };

        await _repo.AddAsync(rec);  // SaveChanges 後可取 rec.Id

        // 2. 以「ID + 副檔名」作為 GCS 物件名
        var objectName = $"{rec.Id}{ext}";

        // 3. 上傳至 Storage（GCS 或 Fake）
        await _storage.UploadAsync(objectName, file.OpenReadStream(), file.ContentType);

        // 此時 FILES 仍保持 Status=processing，縮圖完成後由另一服務更新
        return new(true, "Uploaded");
    }
}
