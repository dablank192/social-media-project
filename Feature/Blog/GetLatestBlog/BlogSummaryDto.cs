using System;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

namespace vsa_w_controller_csharp.Feature.Blog.GetLatestBlog;

public record BlogSummaryDto(
    Guid? Id,
    string? Title,
    List<ImageMetadata>? ImageDetails,
    string? Description,
    DateTime? CreatedAt,
    int LikeCount,
    bool IsLikedByUser
);
