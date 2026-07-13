using System;
using System.ComponentModel.Design;
using Amazon.Runtime;

namespace vsa_w_controller_csharp.Model;

public static class CommentStatus
{
    public const int Active = 0;
    public const int InActive = 1;
}

public class Comment
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public Guid BlogId {get; set;}
    public Guid? ParentCommentId {get; set;}
    public required string Content {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public int IsDelete {get; set;} = CommentStatus.Active;

    public User User {get; set;}
    public Blog Blog {get; set;}
    public List<Comment> Reply {get; set;} = new List<Comment>();
    public Comment? ParentComment {get; set;}
}
