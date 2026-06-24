using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace vsa_w_controller_csharp.Feature.Image;

[ApiController]
[Route("api/v1/vsac/image")]
[Authorize]
public class ImageApi : ControllerBase
{

}
