using AutoMapper;
using Blog.Authorization;
using Blog.Interfaces;
using Blog.Models.PostViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Entities;

namespace Blog.Controllers
{
    public class PostController : DiBaseController
    {
        public PostController(IUnitOfWork unitOfWork, IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager, IMapper mapper) : 
            base(unitOfWork, authorizationService, userManager,
            mapper) { }

        // GET: PostController
        public ActionResult Index(string id)
        {
            var post =  UnitOfWork.Posts.Get(new Guid(id));
            var comments = UnitOfWork.Comments.Find(c => c.PostId == new Guid(id)).ToList();
            post.Comments = comments;
            return View(post);
        }

        // GET: PostController/Create
        public async Task<ActionResult> Create()
        {
            var createNewPostViewModel =  await UnitOfWork.Posts
                .GetCreateNewPostViewModelBy(UserManager.GetUserId(User));
            createNewPostViewModel.Tags = UnitOfWork.Tags.GetAll();
            return View(createNewPostViewModel);
        }

        // POST: PostController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEditPostViewModel viewModel)
        {
            viewModel.UsersBlogs = await UnitOfWork.Blogs.GetBlogsByOwnerId(UserManager.GetUserId(User));
            if (!ModelState.IsValid) return View(viewModel);

            var post = viewModel.Post;
            post.OwnerId = new Guid(UserManager.GetUserId(User));

            var tagIds = viewModel.TagIds.Split("/");
            var tags = tagIds.Select(tagId => UnitOfWork.Tags.Get(new Guid(tagId))).ToList();

            post.Tags = tags;


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
        public async Task<ActionResult> Edit(string id)
        {
            var post = UnitOfWork.Posts.Get(new Guid(id));
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, post,
                ItemOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            var blogs = await UnitOfWork.Blogs.GetBlogsByOwnerId(UserManager.GetUserId(User));
            var viewModel = new CreateEditPostViewModel
            {
                Post = post,
                UsersBlogs = blogs
            };
            
            return View(viewModel);
        }

        // POST: PostController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CreateEditPostViewModel viewModel)
        {
            viewModel.UsersBlogs = await UnitOfWork.Blogs.GetBlogsByOwnerId(UserManager.GetUserId(User));
            if (!ModelState.IsValid) return View(viewModel);
            var postViewModel = viewModel.Post;

            var post = UnitOfWork.Posts.Get(postViewModel.Id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, post,
                ItemOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            try
            {
                post.BlogId = postViewModel.BlogId;
                post.Title = postViewModel.Title;
                post.Body = postViewModel.Body;
                post.Updated = DateTime.Now;
                UnitOfWork.Complete();
                TempData["message"] = $"Your post \"${post.Title}\" has been updated";
                return RedirectToAction("Index", "Post", new { id = post.Id });
            }
            catch
            {
                return View(viewModel);
            }
        }

        // GET: PostController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var post = UnitOfWork.Posts.Get(new Guid(id));
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, post,
                ItemOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            try
            {
                UnitOfWork.Posts.Remove(post);
                UnitOfWork.Complete();
                TempData["message_delete"] = $"Blog with the name \"{post.Title}\" has been deleted";
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                TempData["message_delete"] = $"Something went wrong! \"{post.Title}\" has not been deleted";
                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}