using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
                                //(to save in the back end)
public class AccountController(DataContext context , ITokenService tokenService ,
 IMapper mapper) : BaseApiController
{
[HttpPost("register")]  //end point //account // register
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
{
    //creat a unic username (one user can tack the usernem)
    if(await UserExists(registerDto.Username)) return BadRequest("Username is taken");
    using var hmac = new HMACSHA512();

    var user = mapper.Map<AppUser>(registerDto);
    
    user.UserName = registerDto.Username.ToLower();
    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
    user.PasswordSalt = hmac.Key;


   context.Users.Add(user);
   //save in the data base
   await context.SaveChangesAsync();
   return new UserDto{
    Username = user.UserName ,
    Token = tokenService.CreateToken(user),
    KnownAs= user.KnownAs,
    Gender = user.Gender,
   };
}
//login endpoint
[HttpPost("login")]
public async Task<ActionResult<UserDto>>Login(LoginDto loginDto)
{ 
    var user=await context.Users
    .Include(p => p.photos)
        .FirstOrDefaultAsync(x =>
            x.UserName == loginDto.Username.ToLower());

    if (user == null) return Unauthorized("Invalid username");
    //convert the enteing pass to salt
    using var hmac = new HMACSHA512(user.PasswordSalt);
    //convert the salt pass to hash
    var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

    for(int i =0; i<computeHash.Length; i++)
    {
        if(computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid passwod");
    }
    return new UserDto
    {
        Username = user.UserName ,
        KnownAs =user.KnownAs ,
        Token= tokenService.CreateToken(user),
        Gender = user.Gender,
        PhotoUrl = user.photos.FirstOrDefault(x => x.IsMain)?.Url
    };
} 


//creat a unic username (one user can tack the usernem)
private async Task<bool> UserExists(string username)
{
    return await context.Users.AnyAsync(x=> x.UserName.ToLower()== username.ToLower());
}
}
