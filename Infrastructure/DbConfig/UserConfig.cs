using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Infrastructure.DbConfig;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserName)
        .HasMaxLength(50)
        .IsRequired();
        builder.HasIndex(t => t.UserName);

        builder.Property(t => t.Password)
        .HasMaxLength(100)
        .IsRequired();
    }
}
