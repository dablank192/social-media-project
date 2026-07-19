using System;

namespace vsa_w_controller_csharp.Exception.UserException;

public class UserProfileNotFoundException : System.Exception
{
    public UserProfileNotFoundException() : base(
        message: "Profile Error: User Profile not found exception"
    ){}
}
