using System;
using Amazon.S3;
using Amazon.S3.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace vsa_w_controller_csharp.Feature.Image.UploadImage;

public record ImageDetail(string StorageUrl, string StorageKey);

public record Command(List<string> FileFormat) : IRequest<Result>;
public record Result(
    List<ImageDetail> ImageDetail);

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
    IAmazonS3 s3Client
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle (Command req, CancellationToken ct)
    {
        var ImageDetail = new List<ImageDetail>();
        
        var customFolderPath = $"path/{DateTime.Now:yyyy/MM}";

        
        foreach(var file in req.FileFormat)
        {
            var uniqueFileName = $"{Guid.NewGuid()}.{file}";
            var storageKey = $"{customFolderPath}/{uniqueFileName}";

            var preSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = "products",
                Key = storageKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };

            string signedUploadUrl = s3Client.GetPreSignedURL(preSignedUrlRequest);

            ImageDetail.Add(new ImageDetail(StorageUrl: signedUploadUrl, StorageKey: storageKey));
        }

        return new Result(ImageDetail);

        //chưa test
    }
}