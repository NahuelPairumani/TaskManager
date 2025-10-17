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
    public class FileAssignmentConfiguration : IEntityTypeConfiguration<FileAssignment>
    {
        public void Configure(EntityTypeBuilder<FileAssignment> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__FileAssi__207F322D3BAA6F80");

            builder.ToTable("FileAssignment");

            builder.HasOne(d => d.File).WithMany(p => p.FileAssignments)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FileAssig__FileI__5812160E");

            builder.HasOne(d => d.Task).WithMany(p => p.FileAssignments)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FileAssig__TaskI__571DF1D5");
        }

    }
}
