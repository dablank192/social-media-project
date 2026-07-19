using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.User.UserProfile.UpdateUserProfile;

public record Command(
    Guid UserId,
    SubCommand Body

) : IRequest<Result>;

public record SubCommand(
    string? FirstName,
    string? MiddleName,
    string? LastName,
    string? PhoneNumber,
    string? ContactEmail,
    string? HeadLine,
    string? Bio,

    [RegularExpression(
        pattern: @"^(https?:\/\/)?([\w\d\-_]+\.)+[a-zA-Z]{2,}(\/.*)?$",
        ErrorMessage = "Invalid Url format(eg: https://github.com/username)"
    )]
    string? PortfolioWebsiteUrl
);

public record Result(
    string Message
);

public class UpdateUserProfile(
    ISender sender
) : UserProfileApi

{
    [HttpPut("update")]
    public async Task<IActionResult> HandleAsync([FromBody] SubCommand req)
    {
        var currentUser = User.FindFirst("userid")?.Value;
        if(currentUser == null) throw new UserIdNotFoundException();

        Guid.TryParse(currentUser, out Guid currentUserId);

        var result = await sender.Send(new Command(
            UserId: currentUserId,
            Body: req
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
        var userProfile = await dbContext.UserProfile.FirstOrDefaultAsync(t => t.UserId == req.UserId, ct)
        ?? throw new UserIdNotFoundException();

        userProfile.FirstName = req.Body.FirstName;
        userProfile.LastName = req.Body.LastName;
        userProfile.MiddleName = req.Body.MiddleName;
        userProfile.ContactEmail = req.Body.ContactEmail;
        userProfile.PhoneNumber = req.Body.PhoneNumber;
        userProfile.Bio = req.Body.Bio ?? userProfile.Bio;
        userProfile.HeadLine = req.Body.HeadLine;
        userProfile.PortfolioWebsiteUrl = req.Body.PortfolioWebsiteUrl;

        await dbContext.SaveChangesAsync(ct);

        return new Result(Message: "User's Profile Updated successfully");
    }
}
