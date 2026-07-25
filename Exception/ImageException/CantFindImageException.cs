using System;

namespace vsa_w_controller_csharp.Exception.ImageException;

public class CannotFindImageException : System.Exception
{
    public CannotFindImageException() : base(
        message: "ERROR: Can't not identify image data while uploading"
    ){}
}
