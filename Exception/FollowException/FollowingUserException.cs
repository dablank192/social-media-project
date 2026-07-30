using System;

namespace vsa_w_controller_csharp.Exception.FollowException;

public class FollowingUserException : System.Exception
{
    public FollowingUserException(Guid? userId) : base(
        message: $"ERROR: An error occur while try to follow a user - {userId}"
    ){}
}
