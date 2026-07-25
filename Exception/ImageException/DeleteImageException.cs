using System;

namespace vsa_w_controller_csharp.Exception.ImageException;

public class DeleteImageException : System.Exception
{
    public DeleteImageException(string? message) : base(
        message: $"ERROR: An error occur while trying to delete image from Cloudinary - {message}"
    ){}
}
