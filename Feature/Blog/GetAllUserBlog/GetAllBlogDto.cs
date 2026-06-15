using System;

namespace vsa_w_controller_csharp.Feature.Blog.GetAllUserBlog;

public record GetAllBlogDto(
    int PageSize,
    int PageIndex
);
