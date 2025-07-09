using System;

namespace ImageService.Domain;

public class FileRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = default!;
    public Guid UserId { get; set; }  // JWT sub
    public decimal OriSizeKb { get; set; }  // KB
    public int? TargetSize { get; set; }  // KB
    public decimal? CompSizeKb { get; set; }  // KB
    public string? Status { get; set; }  // processing / done / fail
    public string? Format { get; set; }
    public string? ThumbPath { get; set; }
    public DateTime UploadDt { get; set; } = DateTime.Now;
    public DateTime? ResizeDt { get; set; }
}