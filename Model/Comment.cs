using System;
using Amazon.Runtime;

namespace vsa_w_controller_csharp.Model;

public class Comment
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public Guid BlogId {get; set;}
    public Guid? ParentCommentId {get; set;}
    public required string Content {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public User User {get; set;}
    public Blog Blog {get; set;}
    public List<Comment> Reply {get; set;} = new List<Comment>();
    public Comment? ParentComment {get; set;}
}
