using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GraduationProjectApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GraduationProjectApi.Controllers;

[ApiController]
[Route("auth")]
public class UsersController(JwtOptions jwtOptions) : ControllerBase
{
    [HttpPost]
    [Route("login")]
    public ActionResult<string> AuthenticateUser(AuthRequestTest request)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.Key)), SecurityAlgorithms.HmacSha256Signature),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.NameIdentifier, request.Username),
                new (ClaimTypes.MobilePhone, "+20123456789"),
            })
        };
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken); 

        return Ok(accessToken);
    }




}