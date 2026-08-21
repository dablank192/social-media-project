using System;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;

namespace vsa_w_controller_csharp.Feature.Likes;

public class LikeActionWorker(
    LikeActionQueue queue,
    IServiceScopeFactory scopeFactory
) : BackgroundService

{
    // đã xong 2 hàm logic đọc và xóa cho queue, cần xây thêm logic của sơ đồ


    public async Task<LikeActionResult> ProcessLikeAsync(AppDbContext dbContext, LikeActionItem item, CancellationToken ct)
    {
        var validBlog = await dbContext.Blog.FirstOrDefaultAsync(
            t => t.Id == item.BlogId
            && t.Status == BlogStatus.Active,
            ct
        ) ?? throw new BlogNotFoundException();        
        
        var existingLike = await dbContext.BlogLikes.FirstOrDefaultAsync(
            t => t.UserId == item.UserId
            && t.BlogId == item.BlogId
            , ct);

        if (existingLike == null)
        {
            var newLike = new BlogLikes
            {
                UserId = item.UserId,
                BlogId = item.BlogId
            };

            dbContext.BlogLikes.Add(newLike);
            await dbContext.SaveChangesAsync(ct);
        }
        
        var likeCount = await dbContext.BlogLikes.CountAsync(t => t.BlogId == item.BlogId, ct);

        return new LikeActionResult(
            BlogId: item.BlogId,
            IsLike: true,
            LikeCount: likeCount
            );
    }

    public async Task<LikeActionResult> ProcessUnlikeAsync(AppDbContext dbContext, LikeActionItem item, CancellationToken ct)
    {
        var validBlog = await dbContext.Blog.FirstOrDefaultAsync(
            t => t.Id == item.BlogId
            && t.Status == BlogStatus.Active,
            ct
        ) ?? throw new BlogNotFoundException();
        
        
        var existingLike = await dbContext.BlogLikes.FirstOrDefaultAsync(
            t => t.BlogId == item.BlogId
            && t.UserId == item.UserId,
            ct
        );

        if(existingLike != null)
        {
            dbContext.Remove(existingLike);
        }


        await dbContext.SaveChangesAsync(ct);

        var likeCount = await dbContext.BlogLikes.CountAsync(t => t.BlogId == item.BlogId, ct);

        return new LikeActionResult(
            BlogId: item.BlogId,
            IsLike: false,
            LikeCount: likeCount
        );
    }
}
