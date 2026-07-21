using System;
using System.ComponentModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using vsa_w_controller_csharp.Exception.UserException;
using vsa_w_controller_csharp.Feature.Auth.Login;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.User.UserProfile.GetUserProfile;

public record Query(
    Guid UserId,
    Guid CurrentUserId
) : IRequest<Result>;

public record Result(
    Guid UserId,
    string FirstName,
    string LastName,
    string MiddleName,
    string FullName,
    string HeadLine,
    string Bio,
    string AvatarUrl,
    string PhoneNumber,
    string ContactEmail,
    string PortfolioWebsiteUrl
);

public class GetUserProfile(
    ISender sender
) : UserProfileApi

{
    [HttpGet("profile/{UserId}")]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid UserId)
    {
        var currentUser = User.FindFirst("userid")!.Value;
        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Query(UserId: UserId, CurrentUserId: currentUserId));

        return Ok(result);
    }
}

// public class Handler(
//     AppDbContext dbContext
// ) : IRequestHandler<Query, Result>

// {
//     public async Task<Result> Handle(Query qry, CancellationToken ct)
//     {

//     }
// }
