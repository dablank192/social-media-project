using System;

namespace vsa_w_controller_csharp.Model;

public class RefreshToken
{
    public int Id {get; set;}
    public Guid UserId {get; set;}
    public required string Token {get; set;}
    public DateTime ExpiredAt {get; set;}
    public bool IsRevoked {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public User? User {get; set;}
}
