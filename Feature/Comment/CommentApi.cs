using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace vsa_w_controller_csharp.Feature.Comment;

[ApiController]
[Route("api/v1/vsac/comment")]
[Authorize]
public class CommentApi : ControllerBase
{

}
