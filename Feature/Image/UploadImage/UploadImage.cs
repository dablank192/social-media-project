using System;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using vsa_w_controller_csharp.Exception.ImageException;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;


namespace vsa_w_controller_csharp.Feature.Image.UploadImage;


public record Command(List<IFormFile> FileImage, string FolderName) : IRequest<Result>;
public record Result(
    List<ImageMetadata> ImageDetail);

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
    ICldImageManagement cloudinaryClient
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle (Command req, CancellationToken ct)
    {
        var ImageDetail = new List<ImageMetadata>();

        var uploadTask = req.FileImage.Select(async file =>
        {
            try
            {
                var result = await cloudinaryClient.UploadImageToCldAsync(file, req.FolderName);
                ImageDetail.AddRange(result);

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        });

        var finishedTask = await Task.WhenAll(uploadTask);
        
        if(finishedTask.Any(t => t == false)) throw new UploadImageException(null);

        return new Result(ImageDetail);

        //chưa test
    }
}
