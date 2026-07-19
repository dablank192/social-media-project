using System;
using MediatR;
using vsa_w_controller_csharp.Feature.Auth.Register;
using vsa_w_controller_csharp.Infrastructure;

namespace vsa_w_controller_csharp.Feature.User.UserProfile.CreateUserProfile;

public class CreateUserProfile(
    AppDbContext dbContext
) : INotificationHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent ev, CancellationToken ct)
    {
        var newUserProfile = new Model.UserProfile
        {
            UserId = ev.UserId
        };

        dbContext.UserProfile.Add(newUserProfile);
    }
}
