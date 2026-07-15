using System;

namespace vsa_w_controller_csharp.Feature.Comment.GetReply;

public record ReplyDto(
    Guid BlogId,
    Guid CommentId,
    Guid? ParentCommentId,
    string Content,
    DateTime CreatedAt
);
