using System;
using System.Security.Cryptography;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Auth.Refresh;

public record Command(
    string RefreshToken
) : IRequest<Result>;
public record Result(
    string NewAccessToken,
    string NewRefreshToken
);

public class RefreshLogin(
    ISender sender
) : AuthApi

{
    [HttpPost("refresh")]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Handle ()
    {
        var refreshToken = Request.Cookies["x-refresh-token"];

        var result = await sender.Send(new Command(refreshToken!));

        var cookieAccessOption = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        Response.Cookies.Append("x-access-token", result.NewAccessToken, cookieAccessOption);

        var cookieRefreshOption = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(3),
            Path = "api/v1/vsac/auth/refresh"
        };

        Response.Cookies.Append("x-refresh-token", result.NewRefreshToken, cookieRefreshOption);

        return Ok();
    }
}


public class Handler(
    AppDbContext dbContext,
    IAuthHelper helper
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var validRefreshToken = await dbContext.RefreshToken
        .Include(t => t.User)
        .FirstOrDefaultAsync(
            t => t.Token == req.RefreshToken
            && t.ExpiredAt > DateTime.UtcNow
            && t.IsRevoked == false, ct
        )
        ?? throw new InvalidRefreshTokenException();

        validRefreshToken.IsRevoked = true;

        var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var newToken = new RefreshToken
        {
            UserId = validRefreshToken.UserId,
            Token = newRefreshToken,
            ExpiredAt = DateTime.UtcNow.AddDays(3),
            IsRevoked = false
        };
        dbContext.RefreshToken.Add(newToken);
        await dbContext.SaveChangesAsync(ct);

        var newAccessToken = helper.GenerateJwtToken(validRefreshToken.User!);

        return new Result(newAccessToken, newRefreshToken);
    }
}
