using System;
using System.Data.Common;
using System.Net;
using Amazon.S3;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Exception.ImageException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Blog.CreateBlog;

public record Command(
    Guid UserId,
    string Title,
    string? Description,
    string Content,
    List<string>? StorageKey
) : IRequest<Result>;
public record Result(
    Guid BlogId
);

public class UserCreateBlog(
    ISender sender
) : BlogApi

{
    [HttpPost("new-blog")]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    public async Task<IActionResult> HandleAsync([FromBody]CreateBlogDto req)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(
            UserId: currentUserId,
            Title: req.Title,
            Description: req.Description,
            Content: req.Content,
            StorageKey: req.StorageKey
        ));

        return Ok(result);
    }
}


public class Handler(
    AppDbContext dbContext,
    IAmazonS3 s3Client
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var hasImage = req.StorageKey != null && req.StorageKey.Any();

        if(hasImage)
        {
            var validateTask = req.StorageKey.Select(async key =>
            {
                try
                {
                    var request = new Amazon.S3.Model.GetObjectMetadataRequest
                    {
                        BucketName = "products",
                        Key = key
                    };

                    await s3Client.GetObjectMetadataAsync(request, ct);
                    return true;
                }
                catch (Amazon.S3.AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
            });

            var validation = await Task.WhenAll(validateTask);

            if(validation.Any(t => t == false)) throw new UploadImageException();

        }
        

        using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

        try

        {
            var newBlog = new Model.Blog
            {
                UserId = req.UserId,
                Title = req.Title,
                Description = req.Description,
                Content = req.Content,
                Status = "A"
            };

            dbContext.Blog.Add(newBlog);
            await dbContext.SaveChangesAsync(ct);

            if(hasImage)
            {
                for(int i = 0; i < req.StorageKey.Count(); i++)
                {
                    var newImage = new BlogImages
                    {
                        BlogId = newBlog.Id,
                        StorageKey = req.StorageKey[i],
                        DisplayOrder = i + 1
                    };

                    dbContext.BlogImages.Add(newImage);
                }
                await dbContext.SaveChangesAsync(ct);
            }

            await transaction.CommitAsync(ct);
            
            return new Result(BlogId: newBlog.Id);
        }

        catch (System.Exception)
        {
            await transaction.RollbackAsync(ct);
            throw new CreatedPostException();
        }
    }

}
