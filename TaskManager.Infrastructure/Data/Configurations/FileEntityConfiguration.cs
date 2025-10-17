using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Data.Configurations
{
    public class FileEntityConfiguration: IEntityTypeConfiguration<FileEntity>
    {
        public void Configure(EntityTypeBuilder<FileEntity> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__FileEnti__6F0F98BF525C3B24");

            builder.ToTable("FileEntity");

            builder.Property(e => e.FileName)
                .HasMaxLength(150)
                .IsUnicode(false);
            builder.Property(e => e.FileType)
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.FileUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        }
    }
}
