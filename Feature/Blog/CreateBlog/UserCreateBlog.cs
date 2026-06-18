using System;
using System.Data.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Blog.CreateBlog;

public record Command(
    Guid UserId,
    string Title,
    string? Description,
    string Content
) : IRequest<Result>;
public record Result(
    Guid BlogId
);

public class UserCreateBlog(
    ISender sender
) : BlogApi

{
    [HttpPost("new-blog")]
    public async Task<IActionResult> HandleAsync([FromBody]CreateBlogDto req)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(
            UserId: currentUserId,
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
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var newBlog = new Model.Blog
        {
            UserId = req.UserId,
            Title = req.Title,
            Description = req.Description,
            Content = req.Content,
            Status = "A"
        };

        dbContext.Blog.Add(newBlog);
        await dbContext.SaveChangesAsync(ct);

        return new Result(BlogId: newBlog.Id);
    }
}
