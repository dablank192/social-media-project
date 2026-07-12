using System;

namespace vsa_w_controller_csharp.Exception.Comment;

public class CommentNotFoundException : System.Exception
{
    public CommentNotFoundException(Guid? parentCommentId) : base(
        message: $"ERROR: Parent Comment not found on id: {parentCommentId}"
    ) {}
}
