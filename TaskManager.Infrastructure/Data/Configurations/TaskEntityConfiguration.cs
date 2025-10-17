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
    public class TaskEntityConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__TaskEnti__7C6949B140A2E5FE");

            builder.ToTable("TaskEntity");

            builder.Property(e => e.Description).HasMaxLength(500);
            builder.Property(e => e.DueDate).HasColumnType("datetime");
            builder.Property(e => e.Title)
                .HasMaxLength(150)
                .IsUnicode(false);

            builder.HasOne(d => d.Project).WithMany(p => p.TaskEntities)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TaskEntit__Proje__47DBAE45");

            builder.HasOne(d => d.Status).WithMany(p => p.TaskEntities)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TaskEntit__Statu__48CFD27E");
        }
    }
}
