using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Infrastructure.DbConfig;

public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comment");
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.BlogId);
        builder.HasIndex(t => t.ParentCommentId);
        builder.HasIndex(t => t.CreatedAt);
        builder.HasIndex(t => t.UserId);


        builder.HasOne(t => t.ParentComment)
        .WithMany(t => t.Reply)
        .HasForeignKey(t => t.ParentCommentId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.User)
        .WithMany(t => t.Comment)
        .HasForeignKey(t => t.UserId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Blog)
        .WithMany(t => t.Comment)
        .HasForeignKey(t => t.BlogId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
