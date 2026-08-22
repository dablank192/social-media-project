using System;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace vsa_w_controller_csharp.Feature.Likes;

public class LikeActionWorker(
    LikeActionQueue queue,
    IServiceScopeFactory scopeFactory
) : BackgroundService

{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var item in queue.ReadAllAsync(ct))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                var validBlog = await dbContext.Blog.AnyAsync(
                    t => t.Id == item.BlogId
                    && t.Status == BlogStatus.Active,
                    ct
                );
                if (validBlog == false) throw new BlogNotFoundException();


                LikeActionResult result = item.Action switch
                {
                    ActionType.Like => await sender.Send(new LikeBlog.Command(
                                                BlogId: item.BlogId,
                                                UserId: item.UserId
                                            )),
                    ActionType.Unlike => await sender.Send(new UnlikeBlog.Command(
                                                BlogId: item.BlogId,
                                                UserId: item.UserId
                                            )),
                    _ => throw new System.Exception(message: $"Error occured at: {item.Action}"),
                };
                item.Completion.TrySetResult(result);
            }
            catch (System.Exception ex)
            {
                item.Completion.TrySetException(ex);
            }
        }
    }

}
