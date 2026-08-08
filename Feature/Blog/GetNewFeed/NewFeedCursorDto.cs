using System;

namespace vsa_w_controller_csharp.Feature.Blog.GetNewFeed;

public record NewFeedCursorDto
(
    long Time,
    Guid? BlogId
);
