using System;

namespace vsa_w_controller_csharp.Exception.AuthException;

public class InvalidCredentialsException : System.Exception
{
    public InvalidCredentialsException() : base(
        "Auth Error: Can't find user's username or password in the database"
    ){}
}
