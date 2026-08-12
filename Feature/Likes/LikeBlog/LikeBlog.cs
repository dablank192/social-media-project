using System;
using System.ComponentModel.DataAnnotations;
using Amazon.Runtime.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
) : IRequest<Result>;
public record Result(
    string Message
);

public class LikeBlog(
    ISender sender
) : LikeApi

{
    [HttpPost("")]
    public async Task<IActionResult> HandleAsync([FromBody] LikeDto req)
    {   
        var currentUser = User.FindFirst("userid")!.Value;
        Guid.TryParse(currentUser, out Guid userId);

        var result = await sender.Send(new Command(
            BlogId: req.BlogId,
            UserId: userId
        ));

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
        }
        else
        {
            return new Result(
                Message: "User already liked this post"
                );
        }

        await dbContext.SaveChangesAsync(ct);

        return new Result(
            Message: $"Like added to Blog Id"
            );
    }
}
