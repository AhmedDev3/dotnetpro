using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")] // localhost 5001 /api/users / UsersController end point
public class UsersController(DataContext context) : ControllerBase
{
    [HttpGet]
    //https respons to the claint
           //the type of the data
                                             //the name
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){

        var users =await context.Users.ToListAsync();

        return users;
    }
        [HttpGet("{id:int}")] //endPoint  api/users/id=1,2,3
    public async Task<ActionResult<AppUser>> GetUser(int id){

        var user =await context.Users.FindAsync(id);

        if(user == null) return NotFound();

        return user;
    }
}
