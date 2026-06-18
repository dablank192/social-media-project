using System;
using System.Runtime.InteropServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Blog.DeleteABlog;

public record DeleteBlogDto(
    Guid BlogId
);

public record Command(
    Guid UserId,
    Guid BlogId
) : IRequest<Result>;
public record Result(
    string Message
);

public class DeleteBlog(
    ISender sender
) : BlogApi
{
    [HttpDelete("deactive-blog")]
    public async Task<IActionResult> HandleAsync([FromQuery] DeleteBlogDto req)
    {
        var user = User.FindFirst("userid")?.Value;
        Guid.TryParse(user, out Guid userId);

        var result = await sender.Send(new Command(
            UserId: userId,
            BlogId: req.BlogId
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
        var blog = await dbContext.Blog.FirstOrDefaultAsync(t => t.Id == req.BlogId, ct)
        ?? throw new DeleteBlogNotFoundException();

        blog.Status = BlogStatus.InActive;
        await dbContext.SaveChangesAsync(ct);

        return new Result(Message: "Blog's status set inactive successfully");
    }
}
