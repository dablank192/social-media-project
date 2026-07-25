using System;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

namespace vsa_w_controller_csharp.Feature.Blog.GetLatestBlog;

public record BlogSummaryDto(
    string? Title,
    List<ImageMetadata>? ImageDetails,
    string? Description,
    DateTime? CreatedAt
);
