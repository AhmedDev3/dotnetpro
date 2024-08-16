using System;

namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }

    public required string  UserName { get; set; } 
    //store the pass hash
    public required byte[] PasswordHash { get; set; }
    //store the pass salt
    public required byte[] PasswordSalt { get; set; }
}
