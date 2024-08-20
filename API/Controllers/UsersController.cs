using System;
using API.Data;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]                                                  //use the Auto mapper
public class UsersController(IUserRepository userRepository) : BaseApiController // localhost 5001 /api/users / UsersController end point
{   
    [HttpGet]
    //https respons to the claint
           //the type of the data
                                             
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers(){

        var users =await userRepository.GetMemberAsync();

         

        return Ok(users);
    }   
        [HttpGet("{username}")] //endPoint  api/users/id=1,2,3
    public async Task<ActionResult<MemberDto>> GetUser(string username){

        var user =await userRepository.GetMemberAsync(username);

        if(user == null) return NotFound();

        return user;
    }
}
