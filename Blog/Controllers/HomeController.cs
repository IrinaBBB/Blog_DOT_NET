using System;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Blog.Interfaces;

namespace Blog.Controllers
{
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
            return View(blogs);
        }

        public IActionResult Privacy()
        {
            var blog = _unitOfWork.Blogs.Get(new Guid("EFC47B3D-BE7F-4530-7A20-08D97DF7D44C"));
            blog.Name = "Irina";
            _unitOfWork.Complete();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
