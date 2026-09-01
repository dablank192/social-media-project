using System;
using System.ComponentModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Share.ApiResponse;
using vsa_w_controller_csharp.Share.GetFollowDto;

namespace vsa_w_controller_csharp.Feature.Follow.GetFollowee;

public record Query(
    Guid? UserId,
    int PageIndex = 1,
    int PageSize = 1000
) : IRequest<Result>;

public record Result(
    List<GetFolloweeDto> Items,
    int PageIndex,
    int PageSize,
    int TotalRecord,
    int TotalPage
);

public class GetFollowee(
    ISender sender
) : FollowApi

{
    [HttpGet("followee-list")]
    public async Task<IActionResult> HandleAsync([FromQuery] Query qry)
    {
        var result = await sender.Send(qry);

        var pageData = new PageResponse<GetFolloweeDto>(
            Data: result.Items,
            PageSize: result.PageSize,
            PageIndex: result.PageIndex,
            TotalRecord: result.TotalRecord,
            TotalPage: result.TotalPage
        );

        var response = new ApiResponse<PageResponse<GetFolloweeDto>>(
            Error: null,
            Message: "Success",
            Response: pageData
        );

        return Ok(response);
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
        .Where(t => t.FollowerId == qry.UserId)
        .CountAsync(ct);

        var totalPage = (totalRecord + qry.PageSize - 1) / qry.PageSize;

        var followee = await dbContext.UserFollow
        .Where(t => t.FollowerId == qry.UserId)
        .AsNoTracking()
        .Skip(index)
        .Take(qry.PageSize)
        .Select(t => new GetFolloweeDto(
            FolloweeId: t.FolloweeId,
            FirstName: t.Followee.FirstName,
            LastName: t.Followee.LastName,
            AvatarUrl: t.Followee.AvatarUrl,
            PublicId: t.Followee.PublicId
        ))
        .ToListAsync(ct);

        var response = new Result(
            Items: followee,
            PageIndex: qry.PageIndex,
            PageSize: qry.PageSize,
            TotalRecord: totalRecord,
            TotalPage: totalPage
        );

        return response;
    }
}
