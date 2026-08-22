using System;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Exception.Comment;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Comment.CreateComment;

public record Command(
    Guid BlogId,
    string Content,
    Guid? ParentCommentId,
    Guid UserId
) : IRequest<Result>;

public record SubCommand(
    Guid BlogId,
    string Content,
    Guid? ParentCommentId
);
public record Result(
    string Message
);

public class CreateComment(
    ISender sender
) : CommentApi

{
    [HttpPost("new-comment")]
    public async Task<IActionResult> HandleAsync([FromBody] SubCommand sub)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        Guid.TryParse(currentUser, out Guid currentUserId);
        
        var result =  await sender.Send(new Command(
            BlogId: sub.BlogId,
            Content: sub.Content,
            ParentCommentId: sub.ParentCommentId,
            UserId: currentUserId
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
        var parentCommentId = req.ParentCommentId;

        var validBlog = await dbContext.Blog.FirstOrDefaultAsync(
            t => t.Id == req.BlogId
            && t.Status == BlogStatus.Active
            )
        ?? throw new BlogNotFoundException();
        
        if(parentCommentId != null)
        {
            var target = await dbContext.Comment.FindAsync(parentCommentId, ct)
            ?? throw new CommentNotFoundException(parentCommentId);

            if(target.ParentCommentId != null)
            {
                parentCommentId = target.ParentCommentId;
            }
        }

        var newComment = new Model.Comment
        {
            BlogId = req.BlogId,
            UserId = req.UserId,
            Content = req.Content,
            ParentCommentId = parentCommentId
        };

        dbContext.Comment.Add(newComment);
        await dbContext.SaveChangesAsync(ct);

        return new Result(Message: "Created Comment Successfully");
    }
}
