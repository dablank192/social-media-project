using System;

namespace vsa_w_controller_csharp.Model;

public class User
{
    public Guid Id {get; set;}
    public required string UserName {get; set;}
    public required string Password {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime LastUpdate {get; set;} = DateTime.UtcNow;

    public List<Blog> Blog {get; set;}
    public List<RefreshToken> RefreshToken {get; set;}
}
