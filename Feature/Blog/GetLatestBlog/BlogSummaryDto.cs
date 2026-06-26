using System;

namespace vsa_w_controller_csharp.Feature.Blog.GetLatestBlog;

public record BlogSummaryDto(
    string? Title,
    List<string> StorageKey,
    string? Description,
    DateTime? CreatedAt
);
