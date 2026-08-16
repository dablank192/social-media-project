using System;

namespace vsa_w_controller_csharp.Feature.Likes;

public enum ActionType
{
    Like,
    Unlike
}

public record LikeActionItem(
    Guid UserId,
    Guid BlogId,
    ActionType Action,
    TaskCompletionSource<LikeActionResult> Completion
);

public record LikeActionResult(
    Guid BlogId,
    bool IsLike,
    int LikeCount
);