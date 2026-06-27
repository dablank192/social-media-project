using System;

namespace vsa_w_controller_csharp.Feature.Blog.GetLatestBlog;

public record BlogSummaryDto(
    string? Title,
    List<string>? ImageUrl,
    string? Description,
    DateTime? CreatedAt
);
