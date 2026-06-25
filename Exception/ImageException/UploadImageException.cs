using System;

namespace vsa_w_controller_csharp.Exception.ImageException;

public class UploadImageException : System.Exception
{
    public UploadImageException() : base(
        "Error: Unexpected error occur while try to uploading image to storage"
    ){}
}
