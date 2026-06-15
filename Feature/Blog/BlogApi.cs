using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace vsa_w_controller_csharp.Feature.Blog;


[ApiController]
[Route("api/v1/vsac/blog")]
[Authorize]
public class BlogApi : ControllerBase
{

}
