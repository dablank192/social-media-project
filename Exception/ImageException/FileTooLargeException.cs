using System;

namespace vsa_w_controller_csharp.Exception.ImageException;

public class FileTooLargeException : System.Exception
{
    public FileTooLargeException() : base(
        message: "ERROR: File Upload must be less than 5MB"
    ){}
}
