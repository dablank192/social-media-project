using System;

namespace vsa_w_controller_csharp.Exception.BlogException;

public class UpdateBlogNotFoundException : System.Exception
{
    public UpdateBlogNotFoundException() : base(
        "The Blog's Id of the UpdateAUserBlog API not found"
    ) {}
}
