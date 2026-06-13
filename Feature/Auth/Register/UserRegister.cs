using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Auth.Register;

public record Command(
    string Username,
    string Password
) : IRequest<Result>;
public record Result(
    Guid UserId
);

public class UserRegister(
    AppDbContext dbContext,
    ISender sender
) : AuthApi
{
    [HttpPost("/register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Handle ([FromBody] Command req)
    {
        await sender.Send(req);
        return Created();
    }

}

public class Handler(
    AppDbContext dbContext
) : IRequestHandler<Command, Result>

{
    public async Task<Result> Handle(Command req, CancellationToken ct)
    {
        var validUsername = await dbContext.User.FirstOrDefaultAsync(t => t.UserName == req.Username, ct);

        if (validUsername != null)
        {
            throw new DuplicateUsernameException();
        }

        var hash = new PasswordHasher<object>();

        var hashedPassword = hash.HashPassword(new object() ,req.Password);

        var newUser = new User
        {
            UserName= validUsername!.UserName,
            Password= hashedPassword
        };

        dbContext.User.Add(newUser);
        await dbContext.SaveChangesAsync(ct);

        var response = new Result(newUser.Id);

        return response;

        //chua test
    }
}
