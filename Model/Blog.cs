using System;

namespace vsa_w_controller_csharp.Model;

public class Blog
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public required string Title {get; set;}
    public string? Description {get; set;}
    public required string Content {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime LastUpdate {get; set;} = DateTime.UtcNow;

    public User User {get; set;}
}
