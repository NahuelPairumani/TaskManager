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
    public class TeamUserConfiguration : IEntityTypeConfiguration<TeamUser>
    {
        public void Configure(EntityTypeBuilder<TeamUser> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__TeamUser__0AF5EAD3E32F6FD8");

            builder.ToTable("TeamUser");

            builder.HasOne(d => d.Role).WithMany(p => p.TeamUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeamUser__RoleId__403A8C7D");

            builder.HasOne(d => d.Team).WithMany(p => p.TeamUsers)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeamUser__TeamId__3E52440B");

            builder.HasOne(d => d.User).WithMany(p => p.TeamUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeamUser__UserId__3F466844");
        }
    }
}
