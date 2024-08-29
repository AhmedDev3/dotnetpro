using System;
using API.Data;
using API.Entities;
using API.Helpers;

namespace API.interfaces;

public interface ILikesRepository
{
    Task<UserLiked?> GetUserLiked(int sourceUserId, int targetUserId);
    Task<PagedList<MemberDto>> GetUserLikes (LikesParams likesParams);
    Task<IEnumerable<int>> GetCurrentUserLikedIds (int currentUserId);
    void DeleteLike(UserLiked like);
    void AddLike(UserLiked like);
    Task<bool>SaveChanges();
}
