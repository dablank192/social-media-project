using System;
using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;
using vsa_w_controller_csharp.Share.ApiResponse;

namespace vsa_w_controller_csharp.Feature.Comment.GetComment;

public record Command(
    Guid BlogId,
    int PageIndex = 1,
    int PageSize = 1000
) : IRequest<Result>;
public record Result(
    List<GetCommentDto> Response,
    int PageIndex,
    int PageSize,
    int TotalRecord,
    int TotalPage
);

public class GetComment(
    ISender sender
) : CommentApi

{
    [HttpGet("")]
    public async Task<IActionResult> HandleAsync([FromQuery] Command req)
    {
        var result = await sender.Send(req);

        var pageData = new PageResponse<GetCommentDto>(
            Data: result.Response,
            PageSize: result.PageSize,
            PageIndex: result.PageSize,
            TotalRecord: result.TotalRecord,
            TotalPage: result.TotalPage
        );

        var response = new ApiResponse<PageResponse<GetCommentDto>>(
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
        var rootComment = dbContext.Comment
        .AsNoTracking()
        .Where(t => t.ParentCommentId == null
        && t.BlogId == req.BlogId
        && t.IsDelete == CommentStatus.Active
        && t.Blog.Status == BlogStatus.Active);

        var totalRecord = await rootComment.CountAsync(ct);
        var index = (req.PageIndex - 1) * req.PageSize;
        var totalPage = (totalRecord + req.PageSize - 1) / req.PageSize;

        var items = await rootComment.OrderByDescending(t => t.CreatedAt)
        .Skip(index)
        .Take(req.PageSize)
        .Select(t => new GetCommentDto(
            BlogId: t.BlogId,
            CommentId: t.Id,
            OwnerId: t.UserId,
            Content: t.Content,
            CreatedAt: t.CreatedAt,
            ReplyCount: t.Reply.Count(
                r => r.IsDelete == CommentStatus.Active
                && r.ParentCommentId == t.Id
                )
        ))
        .ToListAsync(ct);

        var response = new Result(
            Response: items,
            PageIndex: req.PageIndex,
            PageSize: req.PageSize,
            TotalPage: totalPage,
            TotalRecord: totalRecord
        );

        return response;

    }
}
