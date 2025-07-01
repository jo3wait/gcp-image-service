using ImageService.Domain;
using ImageService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly ImageDbContext _db;
    public FileRepository(ImageDbContext db) => _db = db;

    public Task<IEnumerable<FileRecord>> ListAsync(Guid userId, CancellationToken ct = default) =>
        _db.Files.Where(f => f.UserId == userId)
                 .OrderByDescending(f => f.UploadDt)
                 .AsNoTracking()
                 .ToListAsync(ct)
                 .ContinueWith(t => t.Result.AsEnumerable(), ct);

    public async Task AddAsync(FileRecord rec, CancellationToken ct = default)
    {
        _db.Files.Add(rec);
        await _db.SaveChangesAsync(ct);
    }
}