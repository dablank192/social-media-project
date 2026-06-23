using System;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.Blog.UpdateAUserBlog;

public record Command(
    Guid BlogId,
    Guid UserId,
    string? Title,
    string? Description,
    string? Content
) : IRequest<Result>;
public record Result(
    Guid BlogId,
    string Message
);

public class UpdateBlog (
    ISender sender
) : BlogApi

{
    [HttpPatch("edit/{BlogId}")]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Handler([FromRoute]Guid blogId ,[FromBody] UpdateBlogDto req)
    {
        var validUser = User.FindFirst("userid")!.Value;
        Guid.TryParse(validUser, out Guid validUserId);

        var result = await sender.Send(new Command(
            BlogId: blogId,
            UserId: validUserId,
            Title: req.Title,
            Description: req.Description,
            Content: req.Content
        ));

        return Ok(result);
    }
}


public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle (Command req, CancellationToken ct)
    {
        var blog = await dbContext.Blog.FirstOrDefaultAsync(t => t.Id == req.BlogId
        && t.UserId == req.UserId, ct);

        if(blog == null) throw new UpdateBlogNotFoundException();

        blog.Title = req.Title ?? blog.Title;
        blog.Content = req.Content ?? blog.Content;
        blog.Description = req.Description ?? blog.Description;

        await dbContext.SaveChangesAsync(ct);

        var response = new Result(
            BlogId: req.BlogId,
            Message: "Blog has been updated successfully"
        );

        return response;
    }
}
