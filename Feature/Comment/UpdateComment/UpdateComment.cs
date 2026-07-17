using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.Comment;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.Comment.UpdateComment;

public record Command(
    Guid UserId,
    SubCommand Params
) : IRequest<Result>;

public record SubCommand(
    Guid CommentId,
    string? Content
);
public record Result(
    string Message
);

public class UpdateComment(
    ISender sender
) : CommentApi

{
    [HttpPatch("update")]
    public async Task<IActionResult> HandleAsync([FromBody] SubCommand req)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(UserId: currentUserId, Params: req));

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var comment = await dbContext.Comment.FirstOrDefaultAsync(
            t => t.Id == req.Params.CommentId
            && t.UserId == req.UserId, ct)
        ?? throw new CommentNotFoundException(req.Params.CommentId);

        comment.Content = req.Params.Content ?? comment.Content;

        await dbContext.SaveChangesAsync(ct);

        return new Result(Message: "Comment updated successfully");
    }
}
