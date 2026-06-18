using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Infrastructure.DbConfig;

public class BlogConfig : IEntityTypeConfiguration<Blog>
{
    public void Configure (EntityTypeBuilder<Blog> builder)
    {
        builder.ToTable("Blog");
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Title);
        builder.Property(t => t.Title)
        .IsRequired()
        .HasMaxLength(200);

        builder.Property(t => t.Description)
        .HasMaxLength(400);

        builder.Property(t => t.Description)
        .IsRequired();

        builder.Property(t => t.Status)
        .IsRequired()
        .HasMaxLength(1)
        .HasDefaultValue("A");

        builder.HasOne(t => t.User)
        .WithMany(t => t.Blog)
        .HasForeignKey(t => t.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
