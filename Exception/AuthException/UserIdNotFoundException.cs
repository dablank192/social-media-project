using System;

namespace vsa_w_controller_csharp.Exception.AuthException;

public class UserIdNotFoundException : System.Exception
{
    public UserIdNotFoundException() : base(
        message: "Auth Error: User's Id not found!"
    ){}
}
