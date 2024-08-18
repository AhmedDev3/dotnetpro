using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController (DataContext context) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")] // endpoint 
    
    public ActionResult<string> GetAuth () 
    {
        return "secret text" ;
    }

        [HttpGet("not-found")] // endpoint 
    
    public ActionResult<AppUser> GetNotFound () 
    {
        var thing = context.Users.Find(-1);

        if(thing == null) return NotFound();

        return thing ;
    }

        [HttpGet("server-error")] // endpoint 
    
    public ActionResult<AppUser> GetServerError() 
    {
        var thing =context.Users.Find(-1) ?? throw new Exception("A bad thing has happened");
        return thing ;
    }

        [HttpGet("bad-request")] // endpoint 
    
    public ActionResult<string> GetBadRequset() 
    {
        return BadRequest("This was not a good request") ;
    }
}
