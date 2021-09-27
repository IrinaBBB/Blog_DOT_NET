using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Blog.Entities;
using Blog.Interfaces;

namespace Blog.Authorization
{
    public class UserIsCommentOwnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Comment>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserIsCommentOwnerAuthorizationHandler(UserManager<IdentityUser>
            userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                OperationAuthorizationRequirement requirement,
                Comment resource)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            var post = _unitOfWork.Posts.Get(resource.PostId);
            var blog = _unitOfWork.Blogs.Get(new Guid(post.BlogId));


            if (resource.OwnerId == new Guid(_userManager.GetUserId(context.User)) && !blog.Locked)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}