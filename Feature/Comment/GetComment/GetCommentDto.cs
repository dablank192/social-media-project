using System;

namespace vsa_w_controller_csharp.Feature.Comment.GetComment;

public record GetCommentDto
(
    Guid BlogId,
    Guid CommentId,
    Guid OwnerId,
    string Content,
    DateTime CreatedAt,
    int ReplyCount
);
