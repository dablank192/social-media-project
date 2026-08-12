using System;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

namespace vsa_w_controller_csharp.Feature.Blog.GetAllUserBlog;

public record BlogDto(
    Guid? BlogId,
    Guid? UserId,
    List<ImageMetadata>? ImageDetails,
    string? Title,
    string? Description,
    string? Content,
    string Status,
    int LikeCount,
    bool IsLikedByUser,
    DateTime? CreatedAt
);
