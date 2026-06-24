using System;
using vsa_w_controller_csharp.Infrastructure.DbConfig;

namespace vsa_w_controller_csharp.Model;

public static class BlogStatus
{
    public const string Active = "A";
    public const string InActive = "I";
}

public class Blog
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public required string Title {get; set;}
    public string? Description {get; set;}
    public required string Content {get; set;}
    public required string Status {get; set;} = BlogStatus.Active;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime LastUpdate {get; set;} = DateTime.UtcNow;

    public User User {get; set;}
    public List<BlogImages> BlogImages {get; set;}
}
