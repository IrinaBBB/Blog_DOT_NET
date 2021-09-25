using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Blog.Entities;

namespace Blog.Authorization
{
    public class UserIsPostOwnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Post>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserIsPostOwnerAuthorizationHandler(UserManager<IdentityUser>
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                OperationAuthorizationRequirement requirement,
                Post resource)
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

            if (resource.OwnerId == new Guid(_userManager.GetUserId(context.User)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}