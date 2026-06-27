using System;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.Image.GetImage;

public record GetImageDto(
    Guid BlogId,
    Guid? ImageId
);

public record Query(
    Guid UserId,
    GetImageDto Params
) : IRequest<Result>;
public record Result(
    List<string>? ImageUrl
);

public class GetImage(
    ISender sender
) : ImageApi

{
    [HttpGet("/view")]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    public async Task<IActionResult> HandleAsync([FromQuery] GetImageDto qry)
    {
        var currentUser = User.FindFirst("userid").Value;
        Guid.TryParse(currentUser, out Guid currentUserId);
        
        var result = await sender.Send(new Query(
            UserId: currentUserId,
            Params: qry
        ));

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext,
    IConfiguration config
) : IRequestHandler<Query, Result>

{
    public async Task<Result> Handle(Query req, CancellationToken ct)
    {
        var imageUrl = new List<string>();
        var s3Endpoint = config.GetSection("S3Storage")["Endpoint"];

        var imageKey = await dbContext.BlogImages
        .Where(t => t.BlogId == req.Params.BlogId)
        .OrderBy(t => t.DisplayOrder)
        .Select(t => t.StorageKey)
        .ToListAsync(ct);

        foreach(var key in imageKey)
        {
            var url = $"{s3Endpoint}/{key}";
            imageUrl.Add(url);
        }

        return new Result(imageUrl);

        //API này dùng để lấy toàn bộ ảnh mà user đã từng đăng, nhưng chưa hoàn thiện do hiện tại project chưa cần dùng tới
    }
}
