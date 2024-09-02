using System;
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser<int>
{
    //this is dont need any more becus we gat them from IdentityUser
    
    // public int Id { get; set; }
    // public required string UserName { get; set; }
    // public byte[] PasswordHash { get; set; } = []; // empty eray
    // public byte[] PasswordSalt { get; set; } = [];// empty eray
    public DateOnly DateOfBirth { get; set; }
    public required string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string Gender { get; set; }
    public string? Inteoduction { get; set; }
    public string? Interest { get; set; }
    public string? LookingFor { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public List<Photo> Photos { get; set; } = [];   // empty eray
    public List<UserLiked> LikedByUsers { get; set; } = [];
     public List<UserLiked> LikedUsers { get; set; } = [];
     //messages
     public List<Message> MessagesSent { get; set; } = [];
     public List<Message> MessagesReceived { get; set; } = [];
     //user role
     public ICollection<AppUserRole> UserRoles { get; set; } = [] ;

}
