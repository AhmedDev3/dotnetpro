using System;
using API.Data;
using API.Entities;

namespace API.interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUserAsync();
    Task<AppUser?> GetUSerByIdAsync(int id);
    Task<AppUser?> GetUSerByUsernameAsync(string username);
    Task<IEnumerable<MemberDto>>GetMemberAsync();
    Task<MemberDto?>GetMemberAsync(string username);
}
