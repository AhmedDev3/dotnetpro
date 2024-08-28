using API.Entities;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext context , IMapper mapper) : IUserRepository
{
    public async Task<PagedList<MemberDto>> GetMemberAsync(UserParams userParams)
    {
        var query= context.Users.AsQueryable();

        query = query.Where(x => x.UserName != userParams.CurrentUsername);

        if(userParams.Gender != null)
        {
            query = query.Where(x => x.Gender == userParams.Gender);
        }

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
        };

        return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider) ,
         userParams.PageNumber,userParams.PageSize);
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
