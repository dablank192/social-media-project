using System;

namespace vsa_w_controller_csharp.Exception.AuthException;

public class DuplicateUsernameException : System.Exception
{
    public DuplicateUsernameException() : base(
        "Auth Error: Duplicate Username"
    ) {}
}
