using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Feature.Blog.GetAllUserBlog;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

namespace vsa_w_controller_csharp.Feature.Blog.GetSglUserBlog;

public record Query(
    Guid BlogId
) : IRequest<Result>;

public record Result(
    BlogDto? Response
);

public class GetSglBlog(
    ISender sender
) : BlogApi

{
    [HttpGet("read/{BlogId}")]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    public async Task<IActionResult> HandleAsync([FromRoute] Query qry)
    {
        var result = await sender.Send(qry);

        return Ok(result);
    }
}


public class Handler(
    AppDbContext dbContext,
    IConfiguration config
) : IRequestHandler<Query, Result>
{
    public async Task<Result> Handle(Query qry, CancellationToken ct)
    {
        var s3Endpoint = config.GetSection("S3Storage")["PublicEndpoint"];
        
        var blog = await dbContext.Blog
        .Where(t => t.Id == qry.BlogId)
        .Select(t => new BlogDto(
            BlogId: t.Id,
            UserId: t.UserId,
            ImageDetails: t.BlogImages
            .OrderBy(t => t.DisplayOrder)
            .Select(t => new ImageMetadata(SecureUrl: t.ImageUrl, PublicId: t.PublicId))
            .ToList(),
            Title: t.Title,
            Description: t.Description,
            Content: t.Content,
            Status: t.Status,
            CreatedAt: t.CreatedAt
        ))
        .FirstOrDefaultAsync(ct);

        var response = new Result(
            Response: blog
        );

        return response;
    }
}
