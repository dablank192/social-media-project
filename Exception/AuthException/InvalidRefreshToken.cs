using System;

namespace vsa_w_controller_csharp.Exception.AuthException;

public class InvalidRefreshTokenException : System.Exception
{
    public InvalidRefreshTokenException() : base(
        "Auth Error: Token is either expired or IsRevoked = true"
    ) {}
}
