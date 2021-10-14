using Microsoft.AspNetCore.Mvc;
using AutoMapper;
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
        protected readonly UserManager<IdentityUser> UserManager;
        protected readonly SignInManager<IdentityUser> SignInManager;
        protected readonly IMapper Mapper;
        protected readonly ITokenService TokenService;

        public DiApiController(
            IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IMapper mapper, ITokenService tokenService, 
            SignInManager<IdentityUser> signInManager)
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
