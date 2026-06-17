using System;

namespace vsa_w_controller_csharp.Feature.Blog.UpdateAUserBlog;

public record UpdateBlogDto (
    string? Title,
    string? Description,
    string? Content
);

