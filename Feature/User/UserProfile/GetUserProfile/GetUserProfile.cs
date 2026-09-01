using System;
using System.ComponentModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.UserException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;
using vsa_w_controller_csharp.Share.ApiResponse;

namespace vsa_w_controller_csharp.Feature.User.UserProfile.GetUserProfile;

public record Query(
    Guid UserId
) : IRequest<Result>;

public record Result(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? MiddleName,
    string? FullName,
    string? HeadLine,
    string? Bio,
    string? AvatarUrl,
    string? PublicId,
    string? PhoneNumber,
    string? ContactEmail,
    string? PortfolioWebsiteUrl,
    ProfileStatus IsPublic
);

public class GetUserProfile(
    ISender sender
) : UserProfileApi

{
    [HttpGet("profile/{UserId}")]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid UserId)
    {
        var result = await sender.Send(new Query(UserId: UserId));

        var response = new ApiResponse<Result>(
            Error: null,
            Message: "Success",
            Response: result
        );

        return Ok(response);
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Query, Result>

{
    public async Task<Result> Handle(Query qry, CancellationToken ct)
    {
        var prf = await dbContext.UserProfile.FirstOrDefaultAsync(
            t => t.UserId == qry.UserId
            && t.IsPublic == ProfileStatus.Public, ct
        )
        ?? throw new UserProfileNotFoundException();

        var response = new Result(
            UserId: prf.UserId,
            FirstName: prf.FirstName,
            MiddleName: prf.MiddleName,
            LastName: prf.LastName,
            FullName: $"{prf.LastName} {prf.FirstName}",
            HeadLine: prf.HeadLine,
            Bio: prf.Bio,
            AvatarUrl: prf.AvatarUrl,
            PublicId: prf.PublicId,
            PhoneNumber: prf.PhoneNumber,
            ContactEmail: prf.ContactEmail,
            PortfolioWebsiteUrl: prf.PortfolioWebsiteUrl,
            IsPublic: prf.IsPublic
        );

        return response;
    }
}
