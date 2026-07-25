using System;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

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
    List<ImageMetadata> ImageDetails
);

public class GetImage(
    ISender sender
) : ImageApi

{
    [HttpGet("/view")]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    public async Task<IActionResult> HandleAsync([FromQuery] GetImageDto qry)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        if(currentUser == null) throw new UserIdNotFoundException();
        
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
    public async Task<Result> Handle(Query req, CancellationToken ct)
    {
        var imgDetails = await dbContext.BlogImages
        .Where(t => t.BlogId == req.Params.BlogId)
        .OrderBy(t => t.DisplayOrder)
        .Select(t => new ImageMetadata(
            SecureUrl: t.ImageUrl,
            PublicId: t.PublicId 
        ))
        .ToListAsync(ct);

        return new Result(ImageDetails: imgDetails);

        //API này dùng để lấy toàn bộ ảnh mà user đã từng đăng, nhưng chưa hoàn thiện do hiện tại project chưa cần dùng tới
    }
}
