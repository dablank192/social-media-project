using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Feature.Auth.Login;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.Blog.GetAllUserBlog;

public record Query(
    Guid UserId,
    GetAllBlogDto Params
) : IRequest<Result>;
public record Result(
    List<BlogDto> Response,
    int PageSize,
    int PageIndex,
    int TotalRecord
);

public class GetAllBlog(
    ISender sender
) : BlogApi

{
    [HttpGet("all-blog")]
    public async Task<IActionResult> HandleAsync([FromQuery] GetAllBlogDto qry)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        Guid.TryParse(currentUser, out Guid currentUserId);
        
        var result = await sender.Send(new Query(
            UserId: currentUserId,
            Params: qry
        ));

        return Ok(result);
    }
}


public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Query, Result>

{
    public async Task<Result> Handle(Query qry, CancellationToken ct)
    {
        var index = (qry.Params.PageIndex - 1) * qry.Params.PageSize;
        var totalRecord = await dbContext.Blog.CountAsync(ct);

        var allBlog = await dbContext.Blog
        .Where(t => t.UserId == qry.UserId) //Need to add condition: status=inactive, but leave at this for for development purpose
        .AsNoTracking()
        .OrderBy(t => t.CreatedAt)
        .Skip(index)
        .Take(qry.Params.PageSize)
        .Select(t => new BlogDto(
            BlogId: t.Id,
            UserId: qry.UserId,
            Title: t.Title,
            Description: t.Description,
            Content: t.Content,
            Status: t.Status,
            CreatedAt: t.CreatedAt
        ))
        .ToListAsync(ct);

        var response = new Result(
            Response: allBlog,
            PageIndex: index,
            PageSize: qry.Params.PageSize,
            TotalRecord: totalRecord
        );

        return response;
    }
}
