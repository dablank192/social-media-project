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
    ISender sender
) : AuthApi
{
    [HttpPost("register")]
    [ProducesResponseType<Result>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Handle ([FromBody] Command req)
    {
        var result = await sender.Send(req);
        return Ok(result);
    }

}

public class Handler(
    AppDbContext dbContext,
    IPublisher publisher
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

        var newUser = new Model.User
        {
            UserName= req.Username,
            Password= hashedPassword
        };

        dbContext.User.Add(newUser);

        await publisher.Publish(new UserRegisteredEvent(newUser.Id), ct);

        await dbContext.SaveChangesAsync(ct);

        var response = new Result(newUser.Id);

        return response;
    }
}
