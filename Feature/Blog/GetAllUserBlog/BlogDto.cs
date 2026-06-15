using System;

namespace vsa_w_controller_csharp.Feature.Blog.GetAllUserBlog;

public record BlogDto(
    Guid? BlogId,
    Guid? UserId,
    string? Title,
    string? Description,
    string? Content,
    DateTime? CreatedAt
);
