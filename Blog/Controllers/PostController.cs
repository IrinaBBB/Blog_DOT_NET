using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Authorization;
using Blog.Interfaces;
using Blog.Models.PostViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    public class PostController : DiBaseController
    {
        public PostController(IUnitOfWork unitOfWork, IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager, IMapper mapper) : 
            base(unitOfWork, authorizationService, userManager,
            mapper) { }

        // GET: PostController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PostController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PostController/Create
        public async Task<ActionResult> Create()
        {
            var createNewPostViewModel =  await UnitOfWork.Posts
                .GetCreateNewPostViewModelBy(UserManager.GetUserId(User));
            return View(createNewPostViewModel);
        }

        // POST: PostController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateNewPostViewModel viewModel)
        {
            viewModel.UsersBlogs = await UnitOfWork.Blogs.GetBlogsByOwnerId(UserManager.GetUserId(User));
            if (!ModelState.IsValid) return View(viewModel);

            var post = viewModel.Post;
            post.OwnerId = new Guid(UserManager.GetUserId(User));

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, post,
                ItemOperations.Create);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            try
            {
                UnitOfWork.Posts.Add(post);
                UnitOfWork.Complete();
                TempData["message"] = $"Your post with title \"{post.Title}\" has been created";
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                return View(viewModel);
            }
        }

        // GET: PostController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PostController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PostController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PostController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}