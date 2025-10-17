﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__User__1788CC4C334F644B");

            builder.ToTable("User");

            builder.HasIndex(e => e.Email, "UQ__User__A9D10534532A0CDD").IsUnique();

            builder.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            builder.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            builder.Property(e => e.PasswordHash)
                .HasMaxLength(200)
                .IsUnicode(false);
        }
    }
}
