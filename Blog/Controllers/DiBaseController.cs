using AutoMapper;
using Blog.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class DiBaseController : Controller
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IAuthorizationService AuthorizationService;
        protected readonly UserManager<IdentityUser> UserManager;
        protected readonly IMapper Mapper;

        public DiBaseController(
            IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager, 
            IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            AuthorizationService = authorizationService;
            UserManager = userManager;
            Mapper = mapper;
        }
    }
}