using System;

namespace vsa_w_controller_csharp.Model;

public class BlogLikes
{
    public int Id {get; set;}
    public Guid BlogId {get; set;}
    public Guid UserId {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public Blog Blog {get; set;}
    public User User {get; set;}
}
