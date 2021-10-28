using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Blog.Entities;
using Blog.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiApiController : ControllerBase
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IAuthorizationService AuthorizationService;
        protected readonly UserManager<ApplicationUser> UserManager;
        protected readonly SignInManager<ApplicationUser> SignInManager;
        protected readonly IMapper Mapper;
        protected readonly ITokenService TokenService;

        public DiApiController(
            IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper, ITokenService tokenService, 
            SignInManager<ApplicationUser> signInManager)
        {
            UnitOfWork = unitOfWork;
            AuthorizationService = authorizationService;
            UserManager = userManager;
            Mapper = mapper;
            TokenService = tokenService;
            SignInManager = signInManager;
        }
    }
}
