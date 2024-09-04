

using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,
    IdentityUserToken<int>>(options)
{
    public DbSet<UserLiked> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups{ get; set; }
    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.Entity<UserLiked>()
        .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        builder.Entity<UserLiked>()
        .HasOne(s => s.SourceUser)
        .WithMany(l => l.LikedUsers)
        .HasForeignKey(s => s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLiked>()
       .HasOne(s => s.TargetUser)
       .WithMany(l => l.LikedByUsers)
       .HasForeignKey(s => s.TargetUserId)
       //if you used sql server you canot use Cascade in bothe use NoAction in one of them
       .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
            .HasOne(x => x.Recipient)
            .WithMany(x => x.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(x => x.Sender)
            .WithMany(x => x.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
