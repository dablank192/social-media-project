using System;

namespace vsa_w_controller_csharp.Feature.Blog.GetLatestBlog;

public record BlogSummaryDto(
    string? Title,
    string? Description,
    DateTime? CreatedAt
);
