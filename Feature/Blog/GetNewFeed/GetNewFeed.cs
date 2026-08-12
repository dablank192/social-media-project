using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.CursorException;
using vsa_w_controller_csharp.Feature.Blog.GetLatestBlog;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;
using vsa_w_controller_csharp.Share.CursorPagination;

namespace vsa_w_controller_csharp.Feature.Blog.GetNewFeed;

public record Query(
    Guid UserId,
    string? Cursor,
    int Limit = 20
) : IRequest<Result>;

public record SubQuery(
    int Limit,
    string Cursor
);

public record Result(
    List<BlogSummaryDto> Items,
    int Limit,
    string NextCursor
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
            Limit: req.Limit,
            Cursor: req.Cursor
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
        var query = dbContext.Blog
        .Where(t => t.Status == BlogStatus.Active &&
        (t.UserId == req.UserId
        || dbContext.UserFollow.Any(u => u.FollowerId == req.UserId && u.FolloweeId == t.UserId))
        );

        if (req.Cursor != null)
        {
            var getCursor = CursorHelper.Decode<NewFeedCursorDto>(req.Cursor);
            if(getCursor == null) throw new InvalidCursorException();

            var blogTime = new DateTime(getCursor.Time);
            
            query = query.Where(t => t.CreatedAt < blogTime || (t.CreatedAt == blogTime && t.Id < getCursor.BlogId));
        }


        var newFeed = await query
        .AsNoTracking()
        .OrderByDescending(t => t.CreatedAt)
        .ThenByDescending(t => t.Id)
        .Take(req.Limit)
        .Select(t => new BlogSummaryDto(
            Id: t.Id,
            Title: t.Title,
            ImageDetails: t.BlogImages
            .OrderBy(t => t.DisplayOrder)
            .Select(t => new ImageMetadata(SecureUrl: t.ImageUrl, PublicId: t.PublicId))
            .ToList(),
            Description: t.Description,
            CreatedAt: t.CreatedAt,
            LikeCount: t.BlogLikes.Count,
            IsLikedByUser: t.BlogLikes.Any(u => u.UserId == req.UserId)
        ))
        .ToListAsync(ct);

        string nextCursor = null;

        if(newFeed.Count > 0)
        {
            var lastBlog = newFeed.Last();

            var cursorData = new NewFeedCursorDto(
                Time: lastBlog.CreatedAt.Value.Ticks,
                BlogId: lastBlog.Id
            );

            nextCursor = CursorHelper.EncodeCursor(cursorData);
        }

        var response = new Result(
            Items: newFeed,
            Limit: req.Limit,
            NextCursor: nextCursor
        );

        return response;
        //chua test
    }
}
