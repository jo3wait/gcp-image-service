using ImageService.Domain;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Infrastructure.Data;

public class ImageDbContext : DbContext
{
    public ImageDbContext(DbContextOptions<ImageDbContext> opt) : base(opt) { }

    public DbSet<FileRecord> Files => Set<FileRecord>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<FileRecord>(e =>
        {
            e.ToTable("FILES");
            e.HasKey(f => f.Id).HasName("PK_FILES");

            e.Property(f => f.Id).HasColumnName("ID").HasDefaultValueSql("NEWSEQUENTIALID()");
            e.Property(f => f.FileName).HasColumnName("FILE_NAME").HasMaxLength(255).IsRequired();
            e.Property(f => f.UserId).HasColumnName("USER_ID").IsRequired();
            e.Property(f => f.OriSizeKb).HasColumnName("ORI_SIZE").HasColumnType("DECIMAL(10,1)").IsRequired();
            e.Property(f => f.TargetSize).HasColumnName("TARGET_SIZE").HasColumnType("INT");
            e.Property(f => f.CompSizeKb).HasColumnName("COMP_SIZE").HasColumnType("DECIMAL(10,1)");
            e.Property(f => f.Status).HasColumnName("STATUS").HasMaxLength(15);
            e.Property(f => f.Format).HasColumnName("FORMAT").HasMaxLength(10);
            e.Property(f => f.ThumbPath).HasColumnName("THUMB_PATH").HasMaxLength(255);
            e.Property(f => f.UploadDt).HasColumnName("UPLOAD_DT").HasDefaultValueSql("SYSDATETIME()");
            e.Property(f => f.ResizeDt).HasColumnName("RESIZE_DT");

            e.HasIndex(f => new { f.UserId, f.UploadDt });
        });
    }
}
