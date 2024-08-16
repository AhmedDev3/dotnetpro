using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
                                //(to save in the back end)
public class AccountController(DataContext context , ITokenService tokenService) : BaseApiController
{
[HttpPost("register")]  //end point //account // register
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
{
    //creat a unic username (one user can tack the usernem)
    if(await UserExists(registerDto.Username)) return BadRequest("Username is taken");

    using var hmac = new HMACSHA512();
   var user = new AppUser {
     UserName = registerDto.Username.ToLower() ,
     PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
     PasswordSalt=hmac.Key
   };
   context.Users.Add(user);
   //save in the data base
   await context.SaveChangesAsync();
   return new UserDto{
    Username = user.UserName ,
    Token = tokenService.CreateToken(user)
   };
}
//login endpoint
[HttpPost("login")]
public async Task<ActionResult<UserDto>>Login(LoginDto loginDto)
{
    var user=await context.Users.FirstOrDefaultAsync(x =>
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
        Token= tokenService.CreateToken(user)
    };
} 


//creat a unic username (one user can tack the usernem)
private async Task<bool> UserExists(string username)
{
    return await context.Users.AnyAsync(x=> x.UserName.ToLower()== username.ToLower());
}
}
