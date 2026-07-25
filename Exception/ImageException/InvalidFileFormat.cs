using System;

namespace vsa_w_controller_csharp.Exception.ImageException;

public class InvalidFileFormat : System.Exception
{
    public InvalidFileFormat() : base(
        message: "ERROR: Invalid File Format (can only use the following: .jpeg, .png, .jpg, .webp)"
    ){}
}
