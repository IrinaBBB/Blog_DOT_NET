using AutoMapper;
using Blog.Entities;
using Blog.Hubs;
using Blog.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Blog.Controllers
{
    public class DiBaseController : Controller
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IAuthorizationService AuthorizationService;
        protected readonly UserManager<ApplicationUser> UserManager;
        protected readonly IMapper Mapper;
        

        public DiBaseController(
            IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager, 
            IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            AuthorizationService = authorizationService;
            UserManager = userManager;
            Mapper = mapper;
        }
    }
}