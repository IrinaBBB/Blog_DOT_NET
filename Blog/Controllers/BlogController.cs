using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Authorization;
using Blog.Entities;
using Microsoft.AspNetCore.Mvc;
using Blog.Interfaces;
using Blog.Models.BlogViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    public class BlogController : DiBaseController
    {

        public BlogController(IUnitOfWork unitOfWork, 
            IAuthorizationService authorizationService, 
            UserManager<ApplicationUser> userManager, IMapper mapper) 
            : base(unitOfWork, authorizationService, userManager, mapper) { }

        // GET: BlogController
        public async Task<ActionResult> Index(string id)
        {
            var blog = await UnitOfWork.Blogs.GetBlogWithPosts(id);
            var posts = UnitOfWork.Posts.Find(p => p.BlogId == id).ToList();
            blog.Posts = posts;
            return View(blog);
        }

        // GET: BlogController/Create
        public ActionResult Create()
        {

            return View(new CreateBlogViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateBlogViewModel blogViewModel)
        {
            if (!ModelState.IsValid) return View(blogViewModel);

            var blog = Mapper.Map<Entities.Blog>(blogViewModel);
            blog.OwnerId = new Guid(UserManager.GetUserId(User));


            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, blog,
                ItemOperations.Create);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            try
            {
                UnitOfWork.Blogs.Add(blog);
                UnitOfWork.Complete();
                TempData["message"] = $"Blog with the name \"{blog.Name}\" has been created";
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                return View(blogViewModel);
            }
        }


        // GET: BlogController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var blog = UnitOfWork.Blogs.Get(new Guid(id));
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, blog,
                ItemOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(blog);
        }

        // POST: BlogController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Entities.Blog blogViewModel)
        {
            if (!ModelState.IsValid) return View(blogViewModel);


            var blog = UnitOfWork.Blogs.Get(blogViewModel.Id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, blog,
                ItemOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            try
            {
                blog.Name = blogViewModel.Name;
                blog.Description = blogViewModel.Description;
                blog.Locked = blogViewModel.Locked;
                blog.Updated = DateTime.Now;
                UnitOfWork.Complete();
                TempData["message"] = $"Your blog \"{blog.Name}\" has been updated";
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                TempData["message_delete"] = $"Something unexpected happened! Your blog \"${blog.Name}\" has not been updated";
                return RedirectToAction("Index", "Dashboard");
            }
        }

        // GET: BlogController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var blog = UnitOfWork.Blogs.Get(new Guid(id));
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, blog,
                ItemOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            var posts = await UnitOfWork.Posts.GetBlogsPosts(id);

            try
            {
                UnitOfWork.Blogs.Remove(blog);
                UnitOfWork.Posts.RemoveRange(posts);
                UnitOfWork.Complete();
                TempData["message_delete"] = $"Blog with the name \"{blog.Name}\" has been deleted";
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                TempData["message_delete"] = $"Something went wrong! \"{blog.Name}\" has not been deleted";
                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}