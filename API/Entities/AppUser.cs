using System;
using API.Extensions;

namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }

    public required string  UserName { get; set; } 
    //store the pass hash
    public  byte[] PasswordHash { get; set; } = []; // empty eray
    //store the pass salt
    public  byte[] PasswordSalt { get; set; } = [] ;// empty eray
    public DateOnly DateOfBirth { get; set; }
    public required string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow ;
    public DateTime LastActive { get; set; }=DateTime.UtcNow ;
    public required string Gender { get; set; }
    public  string? Inteoduction { get; set; }
    public string? Interest { get; set; }
    public string? LookingFor { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public List<Photo> photos { get; set; } = [];   // empty eray

//     public int GetAge()
//     {
//         return DateOfBirth.CalculateAge();
//     }
 }
