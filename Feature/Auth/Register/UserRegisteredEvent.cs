using System;
using MediatR;

namespace vsa_w_controller_csharp.Feature.Auth.Register;

public record UserRegisteredEvent(
    Guid UserId
) : INotification;