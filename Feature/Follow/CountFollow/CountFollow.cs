using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.Follow.CountFollow;

public record Query(Guid UserId) : IRequest<Result>;
public record Result(
    Guid UserId,
    int FolloweeCount,
    int FollowerCount
);

public class CountFollow(
    ISender sender
) : FollowApi
{
    [HttpGet("follow-number/{userId}")]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid userId)
    {
        var result = await sender.Send(new Query(userId));

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Query, Result>

{
    public async Task<Result> Handle(Query qry, CancellationToken ct)
    {
        var user = await dbContext.UserProfile.FirstOrDefaultAsync(t => t.UserId == qry.UserId, ct)
        ?? throw new UserIdNotFoundException();

        var response = new Result(
            UserId: qry.UserId,
            FolloweeCount: user.FolloweeCount,
            FollowerCount: user.FollowerCount
        );

        return response;
    }
}
