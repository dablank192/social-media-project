using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Feature.Follow;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.FollowUser;

public record Command(
    Guid FolloweeId, //Id cua nguoi can theo doi
    Guid FollowerId //Id cua user
) : IRequest<Result>;

public record Result(
    string Message
);

public class FollowUser(
    ISender sender
) : FollowApi

{
    [HttpPost("follow/{followeeId}")]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid followeeId)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        if(currentUser == null) throw new UserIdNotFoundException();

        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(FolloweeId: followeeId, FollowerId: currentUserId));

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle (Command req, CancellationToken ct)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

        try
        {
            var newFollow = new UserFollow
            {
                FolloweeId = req.FolloweeId,
                FollowerId = req.FollowerId
            };

            dbContext.UserFollow.Add(newFollow);

            await dbContext.UserFollow
            .Where(t => t.FolloweeId == req.FolloweeId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.FollowerCount == t.FollowerCount += 1), ct);
        }

        //mai code not tinh nang nay
    }
}
