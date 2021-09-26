using System;
using System.Collections.Generic;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using Blog.Entities;
using Blog.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var blogs = _unitOfWork.Blogs.GetAll();
            var posts = _unitOfWork.Posts.GetAll();
            var viewModel = new MainPageViewModel
            {
                Posts = posts,
                Blogs = blogs
            };
            return View(viewModel);
        }

        public IActionResult BlogPosts(string id)
        {
            var blog = _unitOfWork.Blogs.Get(new Guid(id));
            var posts = _unitOfWork.Posts.Find(p => p.BlogId == id);
            blog.Posts = (List<Post>)posts;
            
            return View(blog);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
