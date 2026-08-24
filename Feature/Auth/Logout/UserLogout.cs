using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.Auth.Logout;

public record Command(string RefreshToken) : IRequest<Result>;

public record Result();

public class UserLogout(
    ISender sender
) : AuthApi

{
    [HttpPost("logout")]
    public async Task<IActionResult> HandleAsync()
    {
        var refreshToken = Request.Cookies["x-refresh-token"];

        if(string.IsNullOrEmpty(refreshToken)) return NoContent();

        await sender.Send(new Command(RefreshToken: refreshToken));

        var deleteOption = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "api/v1/vsac/auth/logout"
        };

        Response.Cookies.Delete("x-refresh-token", deleteOption);
        Response.Cookies.Delete("x-access-token", deleteOption);

        return NoContent();
    }
}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var validToken = await dbContext.RefreshToken.FirstOrDefaultAsync(
            t => t.Token == req.RefreshToken
            && t.IsRevoked == false
            && t.ExpiredAt > DateTime.UtcNow,
            ct);

        if (validToken == null)
        {
            return new Result();
        }

        validToken.IsRevoked = true;

        await dbContext.SaveChangesAsync(ct);

        return new Result();
    }
    //chua test
}
