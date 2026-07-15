using System;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.Comment;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Comment.DeleteComment;

public record Command(Guid CommentId, Guid UserId) : IRequest<Result>;
public record Result(string Message);

public class DeleteComment(
    ISender sender
) : CommentApi

{
    [HttpDelete("delete/{commentId}")]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid commentId)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(commentId, currentUserId));

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
            t => t.Id == req.CommentId
            && t.UserId == req.UserId,
            ct);

        if(comment == null) throw new CommentNotFoundException(req.CommentId);

        comment.IsDelete = CommentStatus.InActive;
        await dbContext.SaveChangesAsync(ct);

        return new Result(Message: "Comment deleted successfully!");

        //chua test
    }
}
