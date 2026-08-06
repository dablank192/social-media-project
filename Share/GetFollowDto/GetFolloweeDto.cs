using System;

namespace vsa_w_controller_csharp.Share.GetFollowDto;

public record GetFolloweeDto(
    Guid FolloweeId,
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    string? PublicId
);
