using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Authorization;
using Blog.Entities;
using Blog.Interfaces;
using Blog.Models.CommentViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentsController : DiApiController
    {
        public CommentsController(IUnitOfWork unitOfWork, IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager, IMapper mapper, ITokenService tokenService,
            SignInManager<ApplicationUser> signInManager) : base(unitOfWork, authorizationService, userManager, mapper,
            tokenService, signInManager)
        {
        }



        /// <summary>
        /// Get list of all the comments.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet("{postId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(string postId)
        {
            var commentsDto =
                Mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDto>>(
                    await UnitOfWork.Comments.GetCommentsByPostId(postId));
            foreach (var comment in commentsDto)
            {
                var user = await UserManager.FindByIdAsync(comment.OwnerId.ToString());
                comment.OwnerName = user.UserName;
            }

            return Ok(commentsDto);
        }

        /// <summary>
        /// Create a new comment.
        /// </summary>
        /// <param name="commentDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto commentDto)
        {
            var comment =
                Mapper.Map<CreateCommentDto, Comment>(commentDto);

            comment.OwnerId = new Guid(commentDto.OwnerId);
            comment.Id = new Guid();

            try
            {
                UnitOfWork.Comments.Add(comment);
                UnitOfWork.Complete();
                var commentReturnDto =
                    Mapper.Map<Comment, CommentDto>(comment);
                var user = await UserManager.FindByIdAsync(comment.OwnerId.ToString());
                commentReturnDto.OwnerName = user.UserName;
                return Ok(commentReturnDto);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Subscribe a user to a blog.
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        [HttpPost("subscribeToBlog/{blogId}")]
        public async Task<ActionResult<CommentDto>> SubscribeToBlog(string blogId)
        {
            try
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);
                var blog = UnitOfWork.Blogs.Get(new Guid(blogId));
                if (new Guid(user.Id) != blog.OwnerId)
                {
                    var blogUser = new BlogApplicationUser
                    {
                        BlogId = blog.Id,
                        Blog = blog,
                        OwnerId = user.Id,
                        Owner = user
                    };
                    blog.BlogApplicationUsers.Add(blogUser);
                    UnitOfWork.Complete();
                }

                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Unsubscribe a user from a blog.
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        [HttpPost("unsubscribeToBlog/{blogId}")]
        public async Task<ActionResult<CommentDto>> UnsubscribeToBlog(string blogId)
        {
            try
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);
                var blog = UnitOfWork.Blogs.Get(new Guid(blogId));

                var blogUser = new BlogApplicationUser
                {
                    BlogId = blog.Id,
                    Blog = blog,
                    OwnerId = user.Id,
                    Owner = user
                };

                var userBlog = UnitOfWork.BlogApplicationUsers;
                userBlog.Remove(blogUser);
                UnitOfWork.Complete();

                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// Delete a comment.
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpDelete("{commentId}")]
        public async Task<ActionResult> Delete(string commentId)
        {
            if (commentId == null)
            {
                return BadRequest(ModelState);
            }

            var comment = UnitOfWork.Comments.Get(new Guid(commentId));
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, comment,
                ItemOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return StatusCode(401, "Not Authorized");
            }

            try
            {
                UnitOfWork.Comments.Remove(comment);
                UnitOfWork.Complete();
                return Ok();
            }
            catch
            {
                return StatusCode(500, "ServerError");
            }
        }

        /// <summary>
        /// Edit a comment.
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="commentDto"></param>
        /// <returns></returns>
        [HttpPut("{commentId}")]
        public async Task<ActionResult> Edit(string commentId, EditCommentDto commentDto)
        {
            var comment = UnitOfWork.Comments.Get(new Guid(commentId));

            if (commentDto == null || comment == null)
            {
                return BadRequest(ModelState);
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, comment,
                ItemOperations.Update);


            if (!isAuthorized.Succeeded)
            {
                return StatusCode(401, "Not Authorized");
            }

            try
            {
                comment.Text = commentDto.Text;
                comment.Updated = DateTime.Now;
                UnitOfWork.Complete();
                return Ok();
            }
            catch
            {
                return StatusCode(500, "Server error");
            }
        }
    }
}