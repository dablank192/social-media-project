using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Exception.UserException;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.User.UserProfile.UpdateAvatarImage;

public record Command(
    Guid UserId,
    string AvatarUrl,
    string PublicId
) : IRequest<Result>;
public record SubCommand(
    string AvatarUrl,
    string PublicId
);
public record Result(
    string Message
);

public class UpdateAvatar(
    ISender sender
) : UserProfileApi

{
    [HttpPost("set-avatar")]
    public async Task<IActionResult> HandleAsync([FromBody] SubCommand req)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        if(currentUser == null) throw new UserIdNotFoundException();

        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(
            UserId: currentUserId,
            AvatarUrl: req.AvatarUrl,
            PublicId: req.PublicId
        ));

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle (Command req, CancellationToken ct)
    {
        var profile = await dbContext.UserProfile.FirstOrDefaultAsync(t => t.UserId == req.UserId, ct)
        ?? throw new UserProfileNotFoundException();

        profile.AvatarUrl = req.AvatarUrl;
        profile.PublidId = req.PublicId;

        await dbContext.SaveChangesAsync(ct);

        return new Result(Message: "User Avatar uploaded successfully");

        //chua test
    }
}
