using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Authorization;
using Microsoft.AspNetCore.Http;
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
            UserManager<IdentityUser> userManager, IMapper mapper) 
            : base(unitOfWork, authorizationService, userManager, mapper) { }

        // GET: BlogController
        public async Task<ActionResult> Index(string id)
        {
            var blog = await UnitOfWork.Blogs.GetBlogWithPosts(id);
            var posts = UnitOfWork.Posts.Find(p => p.BlogId == id).ToList();
            blog.Posts = posts;
            return View(blog);
        }

        
        // GET: BlogController/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
            blog.OwnerId = new System.Guid(UserManager.GetUserId(User));

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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BlogController/Edit/5
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

        // GET: BlogController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BlogController/Delete/5
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