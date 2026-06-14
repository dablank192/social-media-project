using System;
using System.Security.Cryptography;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Auth.Login;

public record Command(
    string Username,
    string Password
) : IRequest<Result>;
public record Result(
    Guid UserId,
    string AccessToken,
    string RefreshToken
);

public class UserLogin(
    ISender sender
) : AuthApi

{
    [HttpPost("login")]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Handler ([FromBody]Command req)
    {
        var result = await sender.Send(req);

        var accessCookieOption = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        Response.Cookies.Append("x-access-token", result.AccessToken, accessCookieOption);

        var refreshCookieOption = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(3),
            Path = "api/v1/vsac/auth/refresh",
        };

        Response.Cookies.Append("x-refresh-token", result.RefreshToken, refreshCookieOption);

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
        var validUser = await dbContext.User.FirstOrDefaultAsync(t => t.UserName == req.Username, ct)
        ?? throw new InvalidCredentialsException();

        var validPassword = new PasswordHasher<object>().VerifyHashedPassword(
            new object(),
            hashedPassword: validUser.Password,
            providedPassword: req.Password
            );

        var accessToken = helper.GenerateJwtToken(validUser);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var newRefreshToken = new RefreshToken
        {
            UserId = validUser.Id,
            Token = refreshToken,
            ExpiredAt = DateTime.UtcNow.AddDays(3),
            IsRevoked = false
        };

        dbContext.RefreshToken.Add(newRefreshToken);
        await dbContext.SaveChangesAsync(ct);

        return new Result(
            UserId: validUser.Id,
            AccessToken: accessToken,
            RefreshToken: refreshToken
            );
    }
}
