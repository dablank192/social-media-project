using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Infrastructure.DbConfig;

public class UserFollowConfig : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        builder.ToTable("UserFollow");
        builder.HasKey(t => new
        {
            t.FolloweeId,
            t.FollowerId
        });

        builder.HasOne(t => t.Follower)
        .WithMany(t => t.Following)
        .HasForeignKey(t => t.FollowerId)
        .HasPrincipalKey(t => t.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Followee)
        .WithMany(t => t.Followers)
        .HasPrincipalKey(t => t.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
