using System;
using Amazon.S3;
using Amazon.S3.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.Image;

public record Command(string FileFormat) : IRequest<Result>;
public record Result(
    string StorageUrl,
    string StorageKey);

public class UploadImage(
    ISender sender
) : ImageApi

{
    [HttpPost("upload")]
    public async Task<IActionResult> HandleAsync([FromBody] Command req)
    {
        var result = await sender.Send(req);
        
        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext,
    IAmazonS3 s3Client
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle (Command req, CancellationToken ct)
    {
        var customFolderPath = $"path/{DateTime.Now:yyyy/MM}";
        var uniqueFileName = $"{Guid.NewGuid()}{req.FileFormat}";
        var storageKey = $"{uniqueFileName}/{customFolderPath}";


        var preSignedUrlRequest = new GetPreSignedUrlRequest
        {
            BucketName = "products",
            Key = storageKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(5)
        };

        string signedUploadUrl = s3Client.GetPreSignedURL(preSignedUrlRequest);

        return new Result(
            StorageUrl: signedUploadUrl,
            StorageKey: storageKey
        );

        //chua test
    }
}