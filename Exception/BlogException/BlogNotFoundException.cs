using System;

namespace vsa_w_controller_csharp.Exception.BlogException;

public class BlogNotFoundException : System.Exception
{
    public BlogNotFoundException(): base(
        message: "Blog's id not found"
    ){}
}
