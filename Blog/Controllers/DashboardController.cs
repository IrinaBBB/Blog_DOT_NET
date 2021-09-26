using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Blog.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    public class DashboardController : DiBaseController
    {
        public DashboardController(IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService, UserManager<IdentityUser> userManager, IMapper mapper) : base(
            unitOfWork, authorizationService, userManager, mapper) { }


        // var user = _mapper.Map<UserViewModel>();
        // GET: DashboardController
        public async Task<ActionResult> Index()
        {
            var blogsOfCurrentUser =
                await UnitOfWork.Blogs.GetBlogsByOwnerId(UserManager.GetUserId(User));
            return View(blogsOfCurrentUser);
        }

        public ActionResult GetPost(string id)
        {
            var post = UnitOfWork.Posts.Get(new Guid(id));
            var posts = UnitOfWork.Posts.Find(p => p.BlogId == id).ToList();
            return View(post);
        }

    }
}