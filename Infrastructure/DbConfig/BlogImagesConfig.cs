using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Infrastructure.DbConfig;

public class BlogImagesConfig : IEntityTypeConfiguration<BlogImages>
{
    public void Configure(EntityTypeBuilder<BlogImages> builder)
    {
        builder.ToTable("BlogImages");
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Blog)
        .WithMany(t => t.BlogImages)
        .HasForeignKey( t => t.BlogId)
        .OnDelete(DeleteBehavior.Restrict);

    }
}
