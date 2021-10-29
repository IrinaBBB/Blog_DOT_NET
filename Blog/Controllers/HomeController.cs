using Blog.Entities;
using Blog.Interfaces;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    [AllowAnonymous]
    public class HomeController : DiBaseController
    {
        public HomeController(IUnitOfWork unitOfWork, IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager, IMapper mapper) : base(unitOfWork, authorizationService,
            userManager, mapper)
        {
        }

        public IActionResult Index()
        {
            var blogs = UnitOfWork.Blogs.GetAll();
            var posts = UnitOfWork.Posts.GetAll();
            var tags = UnitOfWork.Tags.GetAll();

            var viewModel = new MainPageViewModel
            {
                Posts = posts,
                Blogs = blogs,
                Tags = tags
            };
            return View(viewModel);
        }

        public IActionResult TagPosts(string tagId)
        {
            var posts = UnitOfWork.Tags.GetPostsByTag(tagId).Result;
            var blogs = UnitOfWork.Blogs.GetAll();
            var tags = UnitOfWork.Tags.GetAll();

            var viewModel = new MainPageViewModel
            {
                Posts = posts,
                Blogs = blogs,
                Tags = tags
            };

            return View(viewModel);
        }

        public IActionResult BlogPosts(string id)
        {
            var blog = UnitOfWork.Blogs.Get(new Guid(id));
            var posts = UnitOfWork.Posts.Find(p => p.BlogId == id);
            blog.Posts = (List<Post>)posts;

            return View(blog);
        }

        public async Task<IActionResult> Subscriptions()
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var blogs = await UnitOfWork.Blogs.GetSubscriptionsByUser(user.Id);
            
            return View(blogs);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}