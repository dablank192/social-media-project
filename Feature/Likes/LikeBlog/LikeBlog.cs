using System;
using System.ComponentModel.DataAnnotations;
using Amazon.Runtime.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Likes.LikeBlog;

public record LikeDto(
    // [property: Required(ErrorMessage = "Missing Blog Id")]
    Guid BlogId);
public record Command(
    Guid BlogId,
    Guid UserId
) : IRequest<LikeActionResult>;


public class LikeBlog(
    LikeActionQueue queue
) : LikeApi

{
    [HttpPost("")]
    public async Task<IActionResult> HandleAsync([FromBody] LikeDto req, CancellationToken ct)
    {   

        var currentUser = User.FindFirst("userid")!.Value;
        Guid.TryParse(currentUser, out Guid userId);

        if(currentUser == null) throw new UserIdNotFoundException();

        var result = await queue.EnqueueAsync(
            userId: userId,
            blogId: req.BlogId,
            action: ActionType.Like,
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
            t => t.UserId == req.UserId
            && t.BlogId == req.BlogId
            , ct);

        if (existingLike == null)
        {
            var newLike = new BlogLikes
            {
                UserId = req.UserId,
                BlogId = req.BlogId
            };

            dbContext.BlogLikes.Add(newLike);
            await dbContext.SaveChangesAsync(ct);
        }
        
        var likeCount = await dbContext.BlogLikes.CountAsync(t => t.BlogId == req.BlogId, ct);

        return new LikeActionResult(
            BlogId: req.BlogId,
            IsLike: true,
            LikeCount: likeCount,
            Message: "Blog liked successfully"
            );
    }
}