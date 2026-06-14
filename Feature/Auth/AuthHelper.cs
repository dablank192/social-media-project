using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using vsa_w_controller_csharp.Model;

namespace vsa_w_controller_csharp.Feature.Auth;

public interface IAuthHelper
{
    string GenerateJwtToken(User user);
}

public class AuthHelper(
    IConfiguration config
) : IAuthHelper
{
    public string GenerateJwtToken(User user)
    {
        var secretKey = config.GetSection("Jwt")["Key"];

        var claim = new List<Claim>()
        {
            new ("userid", user.Id.ToString()),
            new ("username", user.UserName)
        };

        var keyToByte = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

        var signingCredential = new SigningCredentials(key: keyToByte, algorithm: SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            claims: claim,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: signingCredential
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
