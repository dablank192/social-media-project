using System;

namespace vsa_w_controller_csharp.Exception.UserException;

public class InvalidUserProfileStatusException : System.Exception
{
    public InvalidUserProfileStatusException() : base(
        message: "ERROR: Profile status in database is the same as the requested status"
    ){}
}
