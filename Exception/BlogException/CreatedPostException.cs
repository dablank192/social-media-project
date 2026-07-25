using System;

namespace vsa_w_controller_csharp.Exception.BlogException;

public class CreatedPostException : System.Exception
{
    public CreatedPostException(string message) : base(
        $"Error: An exception occur while trying to create a blog with image in Blog table - {message}"
    ){}
}
