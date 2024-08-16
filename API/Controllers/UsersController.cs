using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController(DataContext context) : BaseApiController // localhost 5001 /api/users / UsersController end point
{   
    [AllowAnonymous]
    [HttpGet]
    //https respons to the claint
           //the type of the data
                                             //the name
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){

        var users =await context.Users.ToListAsync();

        return users;
    }   
        [Authorize]
        [HttpGet("{id:int}")] //endPoint  api/users/id=1,2,3
    public async Task<ActionResult<AppUser>> GetUser(int id){

        var user =await context.Users.FindAsync(id);

        if(user == null) return NotFound();

        return user;
    }
}
