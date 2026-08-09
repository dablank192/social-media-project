using System;

namespace vsa_w_controller_csharp.Exception.CursorException;

public class InvalidCursorException : System.Exception
{  
    public InvalidCursorException() : base(
        message: "ERROR: Invalid cursor can not read"
    ){}
}
