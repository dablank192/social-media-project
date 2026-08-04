using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Share.GetFollowDto;

namespace vsa_w_controller_csharp.Feature.Follow.GetFollower;

public record Query(
    Guid? UserId,
    int PageSize = 1000,
    int PageIndex = 1
) : IRequest<Result>;

public record Result(
    List<GetFollowerDto> Items,
    int? PageIndex,
    int? PageSize,
    int? TotalRecord,
    int? TotalPage
);

public class GetFollower(
    ISender sender
) : FollowApi
{
    [HttpGet("follower-list")]
    public async Task<IActionResult> HandleAsync([FromQuery] Query qry)
    {
        var result = await sender.Send(qry);

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Query, Result>
{
    public async Task<Result> Handle(Query qry, CancellationToken ct)
    {
        var index = (qry.PageIndex - 1) * qry.PageSize;
        var totalRecord = await dbContext.UserFollow
        .Where(t => t.FolloweeId == qry.UserId)
        .CountAsync(ct);

        var totalPage = (totalRecord + qry.PageSize - 1) / qry.PageSize;

        var follower = await dbContext.UserFollow
        .Where(t => t.FolloweeId == qry.UserId)
        .AsNoTracking()
        .OrderBy(t => t.Follower.UserId)
        .Skip(index)
        .Take(qry.PageSize)
        .Select(t => new GetFollowerDto(
            FollowerId: t.Follower.UserId,
            FirstName: t.Follower.FirstName,
            LastName: t.Follower.LastName,
            AvatarUrl: t.Follower.AvatarUrl,
            PublicId: t.Follower.PublicId
        ))
        .ToListAsync(ct);


        var response = new Result(
            Items: follower,
            PageIndex: qry.PageIndex,
            PageSize: qry.PageSize,
            TotalRecord: totalRecord,
            TotalPage: totalPage
        );

        return response;
    }
}
