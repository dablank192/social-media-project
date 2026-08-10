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
) : IRequest<Result>;

public record Result(
    string Message
);

public class UnlikeBlog(
    ISender sender
) : LikeApi

{
    [HttpDelete("unlike")]
    public async Task<IActionResult> HandleAsync([FromBody] LikeDto req)
    {
        var currentUser = User.FindFirst("userid")?.Value
        ?? throw new UserIdNotFoundException();
        Guid.TryParse(currentUser, out Guid currentUserId);
        
        var result = await sender.Send(new Command(BlogId: req.BlogId, UserId: currentUserId));

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle(Command req, CancellationToken ct)
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
        else
        {
            return new Result(Message: "Blog has not been liked yet");
        }

        await dbContext.SaveChangesAsync(ct);

        return new Result(Message: "Blog has been unliked");
    }
}
