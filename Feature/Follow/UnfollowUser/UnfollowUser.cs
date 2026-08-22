using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Exception.FollowException;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.Follow.UnfollowUser;

public record Command(
    Guid FollowerId,
    Guid FolloweeId
) : IRequest<Result>;

public record Result(string Message);

public class UnfollowUser(
    ISender sender
) : FollowApi

{
    [HttpPost("unfollow/{followeeId}")]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid followeeId)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        if(currentUser == null) throw new UserIdNotFoundException();

        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(FollowerId: currentUserId, FolloweeId: followeeId));

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

        if(req.FollowerId == req.FolloweeId) throw new FollowingUserException(req.FollowerId);

        try
        {
            var deleteFollow = await dbContext.UserFollow
            .Where(t => t.FollowerId == req.FollowerId
            && t.FolloweeId == req.FolloweeId)
            .ExecuteDeleteAsync(ct);

            if(deleteFollow == 0)
            {
                return new Result(Message: "User was not following this user");
            }

            await dbContext.UserProfile
            .Where(t => t.UserId == req.FolloweeId && t.FollowerCount > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.FollowerCount, t => t.FollowerCount - 1), ct);

            await dbContext.UserProfile
            .Where(t => t.UserId == req.FollowerId && t.FolloweeCount > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.FolloweeCount, t => t.FolloweeCount - 1), ct);

            await dbContext.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return new Result(Message: "Unfollow successfully");
        }
        catch (System.Exception)
        {
            await transaction.RollbackAsync(ct);
            throw new UnfollowingUserException(req.FolloweeId);
        }
    }
}
