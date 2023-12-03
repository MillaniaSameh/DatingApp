﻿using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController : BaseApiController
{
  private readonly IUnitOfWork _unitOfWork;

  public LikesController(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
  {
    likesParams.UserId = User.GetUserId();

    var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);

    Response.AddPaginationHeader(
      new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages)
    );

    return Ok(users);
  }


  [HttpPost("{username}")]
  public async Task<ActionResult> AddLike(string username)
  {
    var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
    if (likedUser == null) return NotFound();

    var sourceUserId = User.GetUserId();
    var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);
    if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

    var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);
    if (userLike != null) return BadRequest("You already liked this user");

    userLike = new UserLike
    {
      SourceUserId = sourceUserId,
      TargetUserId = likedUser.Id
    };

    sourceUser.LikedUsers.Add(userLike);

    if (await _unitOfWork.Complete()) return Ok();

    return BadRequest("Failed to like user");
  }
}
