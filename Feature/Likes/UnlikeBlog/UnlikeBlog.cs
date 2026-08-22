using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Feature.Likes.LikeBlog;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Likes.UnlikeBlog;

public record Command (
    Guid BlogId,
    Guid UserId
) : IRequest<LikeActionResult>;


public class UnlikeBlog(
    LikeActionQueue queue
) : LikeApi

{
    [HttpDelete("unlike")]
    public async Task<IActionResult> HandleAsync([FromBody] LikeDto req ,CancellationToken ct)
    { 
        var currentUser = User.FindFirst("userid")?.Value
        ?? throw new UserIdNotFoundException();
        Guid.TryParse(currentUser, out Guid currentUserId);
        
        var result = await queue.EnqueueAsync(
            userId: currentUserId,
            blogId: req.BlogId,
            action: ActionType.Unlike,
            ct: ct
        );

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, LikeActionResult>

{
    public async Task<LikeActionResult> Handle(Command req, CancellationToken ct)
    {
        var validBlog = await dbContext.Blog.FirstOrDefaultAsync(
            t => t.Id == req.BlogId
            && t.Status == BlogStatus.Active,
            ct
        ) ?? throw new BlogNotFoundException();
        
        
        var existingLike = await dbContext.BlogLikes.FirstOrDefaultAsync(
            t => t.BlogId == req.BlogId
            && t.UserId == req.UserId,
            ct
        );

        if(existingLike != null)
        {
            dbContext.Remove(existingLike);
        }
        else throw new BlogNotFoundException();


        await dbContext.SaveChangesAsync(ct);

        var likeCount = await dbContext.BlogLikes.CountAsync(t => t.BlogId == req.BlogId, ct);

        return new LikeActionResult(
            BlogId: req.BlogId,
            IsLike: false,
            LikeCount: likeCount,
            Message: "Blog unliked successfully"
        );
    }
}
