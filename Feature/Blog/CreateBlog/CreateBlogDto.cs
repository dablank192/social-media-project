using System;

namespace vsa_w_controller_csharp.Feature.Blog.CreateBlog;

public record CreateBlogDto
(
    string Title,
    string? Description,
    string Content
);
