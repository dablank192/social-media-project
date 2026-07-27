using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace vsa_w_controller_csharp.Feature.Follow;

[ApiController]
[Authorize]
[Route("api/v1/vsac/follow")]
public class FollowApi : ControllerBase
{ 

}
