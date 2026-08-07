using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Feature.Blog.GetLatestBlog;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

namespace vsa_w_controller_csharp.Feature.Blog.GetNewFeed;

public record Query(
    Guid UserId,
    int PageIndex = 1,
    int PageSize = 1000
) : IRequest<Result>;

public record SubQuery(
    int PageIndex = 1,
    int PageSize = 1000
);

public record Result(
    List<BlogSummaryDto> Items,
    int PageIndex,
    int PageSize,
    int TotalRecord
);

public class GetNewFeed(
    ISender sender
) : BlogApi
{
    [HttpGet("newfeed")]
    public async Task<IActionResult> HandleAsync([FromQuery] SubQuery req)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Query(
            UserId: currentUserId,
            PageIndex: req.PageIndex,
            PageSize: req.PageSize
        ));

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Query, Result>
{
    public async Task<Result> Handle(Query req, CancellationToken ct)
    {
        int index = (req.PageIndex - 1) * req.PageSize;
        var totalRecord = await dbContext.Blog
        .Where(t => t.UserId == req.UserId
        || dbContext.UserFollow.Any(u => u.FollowerId == req.UserId && u.FolloweeId == t.UserId))
        .CountAsync(ct);

        var newFeed = await dbContext.Blog
        .Where(t => t.UserId == req.UserId
        || dbContext.UserFollow.Any(u => u.FollowerId == req.UserId && u.FolloweeId == t.UserId))
        .AsNoTracking()
        .Skip(index)
        .Take(req.PageSize)
        .OrderByDescending(t => t.CreatedAt)
        .Select(t => new BlogSummaryDto(
            Title: t.Title,
            ImageDetails: t.BlogImages
            .OrderBy(t => t.DisplayOrder)
            .Select(t => new ImageMetadata(SecureUrl: t.ImageUrl, PublicId: t.PublicId))
            .ToList(),
            Description: t.Description,
            CreatedAt: t.CreatedAt
        ))
        .ToListAsync(ct);

        var response = new Result(
            Items: newFeed,
            PageIndex: req.PageIndex,
            PageSize: req.PageSize,
            TotalRecord: totalRecord
        );

        return response;
        //chua test
    }
}
