using System;
using System.Threading.Channels;

namespace vsa_w_controller_csharp.Feature.Likes;

public class LikeActionQueue
{
    private readonly Channel<LikeActionItem> _channel;

    public LikeActionQueue()
    {
        _channel = Channel.CreateBounded<LikeActionItem>(
            new BoundedChannelOptions(1000)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait,
                AllowSynchronousContinuations = false
            }
        );
    }
    
    public async Task<LikeActionResult> EnqueueAsync(LikeActionItem item, CancellationToken ct)
    {
        var completion = new TaskCompletionSource<LikeActionResult>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        await _channel.Writer.WriteAsync(item, ct);

        return await completion.Task.WaitAsync(ct);
    }

    public IAsyncEnumerable<LikeActionItem> ReadAllAsync(CancellationToken ct)
    {
        return _channel.Reader.ReadAllAsync(ct);

    }
}
