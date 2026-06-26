using System;

namespace vsa_w_controller_csharp.Feature.Blog.GetAllUserBlog;

public record BlogDto(
    Guid? BlogId,
    Guid? UserId,
    List<string>? StorageKey,
    string? Title,
    string? Description,
    string? Content,
    string Status,
    DateTime? CreatedAt
);
