using System;
using System.Threading.Channels;

namespace vsa_w_controller_csharp.Feature.Likes;

public class LikeActionQueue
{
    private readonly Channel<LikeActionItem> _channel;

    public LikeActionQueue()
    {
        _channel = Channel.CreateBounded<LikeActionItem>(
            new BoundedChannelOptions(1000) //tạo queue object với cấu hình capacity là 1000
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait, //config này dùng với case mà có lượng task nhiều hơn capacity,
                //nó sẽ yêu cầu các task đợi cho queue có slot thì mới add vào

                AllowSynchronousContinuations = false
            }
        );
    }
    
    public async Task<LikeActionResult> EnqueueAsync(
        Guid userId,
        Guid blogId,
        ActionType action,
        CancellationToken ct)

    {
        var completion = new TaskCompletionSource<LikeActionResult>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        var item = new LikeActionItem(
            UserId: userId,
            BlogId: blogId,
            Action: action,
            Completion: completion
        );

        await _channel.Writer.WriteAsync(item, ct);

        return await completion.Task.WaitAsync(ct);
    }

    public IAsyncEnumerable<LikeActionItem> ReadAllAsync(CancellationToken ct)
    {
        return _channel.Reader.ReadAllAsync(ct);

    }
}
