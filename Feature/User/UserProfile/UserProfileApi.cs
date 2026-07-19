using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace vsa_w_controller_csharp.Feature.User.UserProfile;

[ApiController]
[Route("api/v1/vsac/user")]
[Authorize]
public class UserProfileApi : ControllerBase
{
    
}
