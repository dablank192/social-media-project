using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using vsa_w_controller_csharp.Exception.AuthException;

namespace vsa_w_controller_csharp.Feature.User.UserProfile.GetCurrentUserProfile;

public class GetMyProfile(
    ISender sender
) : UserProfileApi

{
    [HttpGet("profile/me")]
    public async Task<IActionResult> HandleAsync()
    {
        var currentUser = User.FindFirst("userid")?.Value
        ?? throw new UserIdNotFoundException();

        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new GetUserProfile.Query(UserId: currentUserId));

        return Ok(result);
    }
}
