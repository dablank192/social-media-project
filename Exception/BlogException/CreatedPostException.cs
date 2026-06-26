using System;

namespace vsa_w_controller_csharp.Exception.BlogException;

public class CreatedPostException : System.Exception
{
    public CreatedPostException() : base(
        "Error: An exception occur while trying to create a blog with image in Blog table "
    ){}
}
