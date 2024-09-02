using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
public class AccountController(UserManager<AppUser> userManager , ITokenService tokenService ,
 IMapper mapper) : BaseApiController
{
[HttpPost("register")]  //end point //account // register
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
{
    //creat a unic username (one user can tack the usernem)
    if(await UserExists(registerDto.Username)) return BadRequest("Username is taken");

    var user = mapper.Map<AppUser>(registerDto);
    
    user.UserName = registerDto.Username.ToLower();
    //save the user in  the data base
    var result = await userManager.CreateAsync(user , registerDto.Password);

    if(!result.Succeeded) return BadRequest(result.Errors);

   return new UserDto{
    Username = user.UserName ,
    Token = await tokenService.CreateToken(user),
    KnownAs= user.KnownAs,
    Gender = user.Gender,
   };
}
//login endpoint
[HttpPost("login")]
public async Task<ActionResult<UserDto>>Login(LoginDto loginDto)
{ 
    var user=await userManager.Users
    .Include(p => p.Photos)
    .FirstOrDefaultAsync(x =>
            x.NormalizedUserName == loginDto.Username.ToUpper());

    if (user == null || user.UserName == null) return Unauthorized("Invalid username");
    return new UserDto
    {
        Username = user.UserName ,
        KnownAs =user.KnownAs ,
        Token= await tokenService.CreateToken(user),
        Gender = user.Gender,
        PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
    };
} 


//creat a unic username (one user can tack the usernem)
private async Task<bool> UserExists(string username)
{
    return await userManager.Users.AnyAsync(x=> x.NormalizedUserName== username.ToUpper());
}
}
