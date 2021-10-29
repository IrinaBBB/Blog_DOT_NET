using System;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Authorization;
using Blog.Entities;
using Blog.Interfaces;
using Blog.Models.CommentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Blog.Hubs
{
    public class CommentsHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IAuthorizationService AuthorizationService;
        protected readonly UserManager<ApplicationUser> UserManager;
        protected readonly IMapper Mapper;


        public CommentsHub(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            UserManager = userManager;
            AuthorizationService = authorizationService;
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task AddComment(string text, string postId)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var comment = new Comment
                {
                    Id = new Guid(),
                    Text = text,
                    PostId = new Guid(postId),
                    OwnerId = new Guid(UserManager.GetUserId(_httpContextAccessor.HttpContext.User)),
                    Created = DateTime.Now
                };

                var isAuthorized = await AuthorizationService.AuthorizeAsync(
                    _httpContextAccessor.HttpContext.User, comment,
                    ItemOperations.Create);

                if (!isAuthorized.Succeeded)
                {
                    return;
                }

                try
                {
                    UnitOfWork.Comments.Add(comment);
                    UnitOfWork.Complete();
                }
                catch
                {
                    return;
                }

                var commentReturnDto =
                    Mapper.Map<Comment, CommentDto>(comment);
                var user = await UserManager.FindByIdAsync(comment.OwnerId.ToString());
                commentReturnDto.OwnerName = user.UserName;

                await Clients.All.SendAsync("ReceiveComment", commentReturnDto.Id, commentReturnDto.Text, commentReturnDto.OwnerName,
                    commentReturnDto.Created);
            }
        }
    }
}