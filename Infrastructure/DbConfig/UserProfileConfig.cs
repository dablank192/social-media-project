using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Infrastructure.DbConfig;

public class UserProfileConfig : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfile");
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.ContactEmail);
        builder.HasIndex(t => t.PhoneNumber);
        builder.HasIndex(t => t.FirstName);
        builder.HasIndex(t => t.LastName);

        builder.Property(t => t.FirstName)
        .HasMaxLength(100);
        builder.Property(t => t.LastName)
        .HasMaxLength(100);
        builder.Property(t => t.HeadLine)
        .HasMaxLength(120);
        builder.Property(t => t.Bio)
        .HasMaxLength(500);

        builder.HasOne(t => t.User)
        .WithOne(t => t.UserProfile)
        .HasForeignKey<UserProfile>(t => t.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
