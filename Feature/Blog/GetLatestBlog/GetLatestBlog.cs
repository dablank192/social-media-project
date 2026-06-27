using System;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Feature.Blog.GetAllUserBlog;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.Blog.GetLatestBlog;

public record Command(
    DateOnly FromDate,
    DateOnly ToDate,
    int PageIndex,
    int PageSize
) : IRequest<Result>;

public record Result(
    List<BlogSummaryDto> Response,
    int PageIndex,
    int PageSize,
    int TotalRecord
);

public class GetLatestBlog(
    ISender sender
) : BlogApi

{
    [HttpGet("feed")]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Handle([FromQuery] Command req)
    {
        var result = await sender.Send(req);

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext,
    IConfiguration config
) : IRequestHandler<Command, Result>
{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var fromDate = DateTime.SpecifyKind(req.FromDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var toDate = DateTime.SpecifyKind(req.ToDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        var pageIndex = (req.PageIndex - 1) * req.PageSize;
        var totalRecord = await dbContext.Blog.CountAsync(ct);


        var s3Endpoint = config.GetSection("S3Storage")["PublicEndpoint"];

        var blog = await dbContext.Blog.Where(
            t => t.CreatedAt >= fromDate
            && t.CreatedAt <= toDate
        )
        .AsNoTracking()
        .OrderByDescending(t => t.CreatedAt)
        .Skip(pageIndex)
        .Take(req.PageSize)
        .Select(t => new BlogSummaryDto(
            Title: t.Title,
            ImageUrl: t.BlogImages
            .OrderBy(t => t.DisplayOrder)
            .Select(t => s3Endpoint + "/" + t.StorageKey)
            .ToList(),
            Description: t.Description,
            CreatedAt: t.CreatedAt
        ))
        .ToListAsync(ct);

        var response = new Result(
            Response: blog,
            PageIndex: req.PageIndex,
            PageSize: req.PageSize,
            TotalRecord: totalRecord
        );

        return response;
    }
}
