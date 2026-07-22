using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Exception.UserException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.User.UserProfile.SetUserProfileStatus;

public record Command(
    ProfileStatus Status,
    Guid UserId
) : IRequest<Result>;

public record SubCommand(
    ProfileStatus Status
);

public record Result(
    string Message
);

public class SetUserProfileStatus(
    ISender sender
) : UserProfileApi
{
    [HttpPost("profile-visibility")]
    public async Task<IActionResult> HandleAsync([FromBody] SubCommand req)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        if(currentUser == null) throw new UserIdNotFoundException();

        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(
            Status: req.Status,
            UserId: currentUserId
        ));

        return Ok(result);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>
{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var prf = await dbContext.UserProfile.FirstOrDefaultAsync(t => t.UserId == req.UserId, ct)
        ?? throw new UserProfileNotFoundException();


        try
        {
            if (req.Status == ProfileStatus.Private && prf.IsPublic == ProfileStatus.Public)
            {
                prf.IsPublic = ProfileStatus.Private;
            }
            else if (req.Status == ProfileStatus.Public && prf.IsPublic == ProfileStatus.Private)
            {
                prf.IsPublic = ProfileStatus.Public;
            }
            else if(
                (req.Status == ProfileStatus.Public && prf.IsPublic != ProfileStatus.Private)
                ||
                (req.Status == ProfileStatus.Private && prf.IsPublic != ProfileStatus.Public)
            )
            {
                throw new InvalidUserProfileStatusException();
            }

            await dbContext.SaveChangesAsync(ct);
        }
        catch (System.Exception ex)
        {
            throw new System.Exception($"Encounter Unknown Error while try to change User profile's status: {ex}");
        }


        return new Result(Message: "Profile status updated successfully");
    }
}
