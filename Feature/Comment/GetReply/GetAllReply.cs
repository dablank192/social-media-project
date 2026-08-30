using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;
using vsa_w_controller_csharp.Share.ApiResponse;

namespace vsa_w_controller_csharp.Feature.Comment.GetReply;

public record Command(
    Guid CommentId,
    int PageIndex = 1,
    int PageSize = 1000
    ) : IRequest<Result>;
public record Result(
    List<ReplyDto> Items,
    int PageIndex,
    int PageSize,
    int TotalPage,
    int TotalRecord
    );

public class GetAllReply(
    ISender sender
) : CommentApi

{
    [HttpGet("all-reply")]
    public async Task<IActionResult> HandleAsync([FromQuery] Command req)
    {
        var result = await sender.Send(req);

        var pageData = new PageResponse<ReplyDto>(
            Data: result.Items,
            PageIndex: result.PageIndex,
            PageSize: result.PageSize,
            TotalRecord: result.TotalRecord,
            TotalPage: result.TotalPage
        );

        var response = new ApiResponse<PageResponse<ReplyDto>>(
            Error: null,
            Message: "Success",
            Response: pageData
        );

        return Ok(response);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>
{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var root = dbContext.Comment
        .AsNoTracking()
        .Where(t => t.ParentCommentId == req.CommentId
        && t.IsDelete == CommentStatus.Active
        && t.Blog.Status == BlogStatus.Active);

        var totalRecord = await root.CountAsync(ct);
        var index = (req.PageIndex - 1) * req.PageSize;
        var totalPage = (totalRecord + req.PageSize - 1) / req.PageSize;

        var replies = await root
        .Skip(index)
        .Take(req.PageSize)
        .OrderByDescending(t => t.CreatedAt)
        .Select(t => new ReplyDto(
            BlogId: t.BlogId,
            CommentId: t.Id,
            ParentCommentId: t.ParentCommentId,
            Content: t.Content,
            CreatedAt: t.CreatedAt
        ))
        .ToListAsync(ct);

        var response = new Result(
            Items: replies,
            PageIndex: req.PageIndex,
            PageSize: req.PageSize,
            TotalPage: totalPage,
            TotalRecord: totalRecord
        );

        return response;
    }
}
