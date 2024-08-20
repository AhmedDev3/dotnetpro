using System;
using API.Entities;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext context , IMapper mapper) : IUserRepository
{
    public async Task<IEnumerable<MemberDto>> GetMemberAsync()
    {
        return  await context.Users
        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<MemberDto?> GetMemberAsync(string username)
    {
        return await context.Users
        .Where(x => x.UserName == username)
        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async  Task<IEnumerable<AppUser>> GetUserAsync()
    {
        return await context.Users
        .Include(x => x.photos)
        .ToListAsync();
    }

    public async Task<AppUser?> GetUSerByIdAsync(int id)
    {
       return await context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUSerByUsernameAsync(string username)
    {
        return await context.Users
        .Include(x => x.photos)
        .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<bool> SaveAllAsync()
    {
        //اذا اكبر من الصفر يعني اكو داتا اتسيفت في الداتا بيس
        return await context.SaveChangesAsync() > 0 ;
    }

    public void Update(AppUser user)
    {
        context.Entry(user).State= EntityState.Modified ;
    }
}
