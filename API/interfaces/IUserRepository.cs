using System;
using API.Data;
using API.Entities;
using API.Helpers;

namespace API.interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetUserAsync();
    Task<AppUser?> GetUSerByIdAsync(int id);
    Task<AppUser?> GetUSerByUsernameAsync(string username);
    Task<PagedList<MemberDto>>GetMemberAsync(UserParams userParams);
    Task<MemberDto?>GetMemberAsync(string username);
}
