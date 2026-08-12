using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Infrastructure.DbConfig;

public class BlogLikesConfig : IEntityTypeConfiguration<BlogLikes>
{
    public void Configure(EntityTypeBuilder<BlogLikes> builder)
    {
        builder.ToTable("BlogLikes");
        builder.HasKey(t => new
        {
            t.UserId,
            t.BlogId
        });

        builder.HasOne(t => t.Blog)
        .WithMany(t => t.BlogLikes)
        .HasForeignKey(t => t.BlogId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.BlogId);

        builder.HasOne(t => t.User)
        .WithMany(t => t.BlogLikes)
        .HasForeignKey(t => t.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
