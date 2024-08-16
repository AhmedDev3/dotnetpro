using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
       var tokenKey = config["TOkenKey"] ?? throw new Exception("Cannot access tokenKey From appsettings");

       if(tokenKey.Length < 64) throw new Exception ("Your tokenKey needs to be longer");

       var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        //claims => ahmed claim ahmed , diago claim diago
       var claims=new List<Claim>
       {
        new (ClaimTypes.NameIdentifier,user.UserName)
       }; 
        var creds= new SigningCredentials(key , SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            //the token Expir
            Expires = DateTime.UtcNow.AddDays(7),

            SigningCredentials= creds
        };
        
        var tokenHandler =new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
