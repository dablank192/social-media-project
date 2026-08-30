using System;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Feature.Blog.GetAllUserBlog;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Share.ApiResponse;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

namespace vsa_w_controller_csharp.Feature.Blog.GetLatestBlog;

public record SubCommand(
    DateOnly FromDate,
    DateOnly ToDate,
    int PageIndex,
    int PageSize
);

public record Command(
    DateOnly FromDate,
    DateOnly ToDate,
    int PageIndex,
    int PageSize,
    Guid UserId
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
    public async Task<IActionResult> Handle([FromQuery] SubCommand req)
    {
        var currentUser = User.FindFirst("userid")?.Value
        ?? throw new UserIdNotFoundException();
        
        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(
            FromDate: req.FromDate,
            ToDate: req.ToDate,
            PageIndex: req.PageIndex,
            PageSize: req.PageSize,
            UserId: currentUserId
        ));

        var pageData = new PageResponse<BlogSummaryDto>(
            Data: result.Response,
            PageIndex: result.PageIndex,
            PageSize: result.PageSize,
            TotalRecord: result.TotalRecord,
            TotalPage: null
        );

        var response = ApiResponse<PageResponse<BlogSummaryDto>>.Success(
            SuccessMessage: "Success",
            SuccessResponse: pageData
        );

        return Ok(response);
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

        var response = new Result(
            Response: blog,
            PageIndex: req.PageIndex,
            PageSize: req.PageSize,
            TotalRecord: totalRecord
        );

        return response;
    }
}
