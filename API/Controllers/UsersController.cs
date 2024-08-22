using System;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]                                                  //use the Auto mapper
public class UsersController(IUserRepository userRepository , IMapper mapper) : BaseApiController // localhost 5001 /api/users / UsersController end point
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
    [HttpPut]// endpoint to update the user info
    public async Task<ActionResult> UpdateUser (MemberUpdateDto memberUpdateDto)
    {
        var username= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(username == null) return BadRequest("no username found in token ");

        var user = await userRepository.GetUSerByUsernameAsync(username);

        if(user ==  null) return BadRequest("Could not find user");

        mapper.Map(memberUpdateDto , user);

        if(await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }
}
