using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Authorization;
using Blog.Entities;
using Blog.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    public class CommentController : DiBaseController
    {
        public CommentController(IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager, IMapper mapper)
            : base(unitOfWork, authorizationService, userManager, mapper)
        {
        }


        // GET: CommentController/Create
        public ActionResult Create(string id)
        {
            var comment = new Comment
            {
                PostId = new Guid(id)
            };
            return View(comment);
        }

        // POST: CommentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Comment comment)
        {
            if (!ModelState.IsValid) return View(comment);

            comment.OwnerId = new System.Guid(UserManager.GetUserId(User));
            comment.Id = new Guid();

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, comment,
                ItemOperations.Create);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            try
            {
                UnitOfWork.Comments.Add(comment);
                UnitOfWork.Complete();
                TempData["message"] = $"Your comment has been created";
                return RedirectToAction("Index", "Post", new { Id = comment.PostId });
            }
            catch
            {
                return View(comment);
            }
        }

        // GET: CommentController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var comment = UnitOfWork.Comments.Get(new Guid(id));
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, comment,
                ItemOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(comment);
        }

        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Comment commentViewModel)
        {
            if (!ModelState.IsValid) return View(commentViewModel);

            var comment = UnitOfWork.Comments.Get(commentViewModel.Id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, comment,
                ItemOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            try
            {
                comment.Text = commentViewModel.Text;
                comment.Updated = DateTime.Now;
                UnitOfWork.Complete();
                TempData["message"] = "Your comment has been updated";
                return RedirectToAction("Index", "Post", new { Id = comment.PostId });
            }
            catch
            {
                TempData["message_delete"] = $"Something unexpected happened! Your comment has not been updated";
                return View(comment);
            }
        }

        // GET: CommentController/Delete/5
        public async Task<ActionResult> Delete(string commentId, string postId)
        {
            var comment = UnitOfWork.Comments.Get(new Guid(commentId));
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, comment,
                ItemOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            try
            {
                UnitOfWork.Comments.Remove(comment);
                UnitOfWork.Complete();
                TempData["message_delete"] = $"Your comment has been deleted";
                return RedirectToAction("Index", "Post", new { Id = postId });
            }
            catch
            {
                TempData["message_delete"] = $"Something went wrong! Your comment has not been deleted";
                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}