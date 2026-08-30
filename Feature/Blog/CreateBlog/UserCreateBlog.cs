using System;
using System.Net;
using Amazon.S3;
using CloudinaryDotNet;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Exception.ImageException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;
using vsa_w_controller_csharp.Share.ApiResponse;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

namespace vsa_w_controller_csharp.Feature.Blog.CreateBlog;

public record Command(
    Guid UserId,
    string Title,
    string? Description,
    string Content,
    List<ImageMetadata>? ImageDetails
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
            ImageDetails: req.ImageDetails
        ));

        var response = ApiResponse<Result>.Success(
            SuccessMessage: "Blog created successfully",
            SuccessResponse: result
        );

        return Ok(response);
    }
}


public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var hasImage = req.ImageDetails != null && req.ImageDetails.Any();

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
                for(int i = 0; i < req.ImageDetails.Count(); i++)
                {
                    var newImage = new BlogImages
                    {
                        BlogId = newBlog.Id,
                        ImageUrl = req.ImageDetails[i].SecureUrl,
                        PublicId = req.ImageDetails[i].PublicId,
                        DisplayOrder = i + 1
                    };

                    dbContext.BlogImages.Add(newImage);
                }
                await dbContext.SaveChangesAsync(ct);
            }

            await transaction.CommitAsync(ct);
            
            return new Result(BlogId: newBlog.Id);
        }

        catch (System.Exception ex)
        {
            await transaction.RollbackAsync(ct);
            throw new CreatedPostException(ex.ToString());
        }
    }

}
