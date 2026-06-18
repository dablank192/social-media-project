using System;

namespace vsa_w_controller_csharp.Exception.BlogException;

public class DeleteBlogNotFoundException : System.Exception
{
    public DeleteBlogNotFoundException() : base(
        "The Blog's Id of the DeleteAUserBlog API not found"
    ){}
}
