using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Domain;

public interface IFileRepository
{
    Task<IEnumerable<FileRecord>> ListAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(FileRecord rec, CancellationToken ct = default);
}
