using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace vsa_w_controller_csharp.Feature.Likes;

[ApiController]
[Authorize]
[Route("api/v1/vsac/like")]
public class LikeApi : ControllerBase
{
    
}
