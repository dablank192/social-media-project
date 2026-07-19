using System;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> option) : base(option) {}

    public DbSet<User> User {get; set;}
    public DbSet<Blog> Blog {get; set;}
    public DbSet<BlogImages> BlogImages {get; set;}
    public DbSet<RefreshToken> RefreshToken {get; set;}
    public DbSet<BlogLikes> BlogLikes {get; set;}
    public DbSet<Comment> Comment {get; set;}
    public DbSet<UserProfile> UserProfile {get; set;}

    public void Configure(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
