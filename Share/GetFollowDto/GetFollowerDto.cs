using System;

namespace vsa_w_controller_csharp.Share.GetFollowDto;

public record GetFollowerDto(
    Guid FollowerId,
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    string? PublicId
);