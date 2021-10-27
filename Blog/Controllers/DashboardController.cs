using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Entities;
using Microsoft.AspNetCore.Mvc;
using Blog.Interfaces;
using Blog.Models.TagViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    public class DashboardController : DiBaseController
    {
        public DashboardController(IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService, UserManager<IdentityUser> userManager, IMapper mapper) : base(
            unitOfWork, authorizationService, userManager, mapper)
        {
        }


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
            return View(post);
        }

        public ActionResult CreateTag()
        {
            var viewModel = new CreateTagViewModel
            {
                Tag = new Tag(),
                AllTagsList = UnitOfWork.Tags.GetAll()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        private async Task<ActionResult> CreateTag(CreateTagViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var tag = viewModel.Tag;

            if (UnitOfWork.Tags.Find(t => string.Equals(t.Name.ToLower(), viewModel.Tag.Name.ToLower())).Any())
            {
                viewModel.AllTagsList = UnitOfWork.Tags.GetAll();
                TempData["message_delete"] = $"Tag \"{tag.Name}\" already exists";
                return View(viewModel);
            }

            try
            {
                UnitOfWork.Tags.Add(tag);
                UnitOfWork.Complete();
                TempData["message"] = $"Blog with the name \"{tag.Name}\" has been created";
                return RedirectToAction("CreateTag", "Dashboard");
            }
            catch
            {
                return View(viewModel);
            }
        }
    }
}