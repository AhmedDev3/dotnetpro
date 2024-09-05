

using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost("{targetUserId:Int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId= User.GetUserId();

        if(sourceUserId == targetUserId)return BadRequest("You Cannot like yourself");

        var existingLike = await unitOfWork.LikesRepository.GetUserLiked(sourceUserId , targetUserId);

        if(existingLike == null)
        {
            var like = new UserLiked{
            SourceUserId = sourceUserId ,
            TargetUserId = targetUserId ,
        } ;

        unitOfWork.LikesRepository.AddLike(like);
    }
    else
    {
        unitOfWork.LikesRepository.DeleteLike(existingLike);
    }

    if (await unitOfWork.Complete()) return Ok();

    return BadRequest("Faild to update like");
     }
     
     [HttpGet("list")]
     public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
     {
        return Ok(await unitOfWork.LikesRepository.GetCurrentUserLikedIds(User.GetUserId()));
     }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes ([FromQuery]LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await unitOfWork.LikesRepository.GetUserLikes(likesParams);

        Response.AddPaginationHeader(users);

        return Ok(users);
    }
}
