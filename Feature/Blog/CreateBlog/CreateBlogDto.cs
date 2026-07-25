using System;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

namespace vsa_w_controller_csharp.Feature.Blog.CreateBlog;

public record CreateBlogDto
(
    string Title,
    string? Description,
    string Content,
    List<ImageMetadata>? ImageDetails
);
