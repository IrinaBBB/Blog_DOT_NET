using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Entities;
using Blog.Interfaces;
using Blog.Models.AccountDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [AllowAnonymous]
    public class AccountController : DiApiController
    {
        public AccountController(IUnitOfWork unitOfWork, IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager, IMapper mapper, ITokenService tokenService,
            SignInManager<ApplicationUser> signInManager) : base(unitOfWork, authorizationService, userManager, mapper,
            tokenService, signInManager)
        {
        }

        [HttpPost("login")]
        public async Task<ActionResult<IdentityUserDto>> Login(LoginDto loginDto)
        {
            var user = await UserManager.Users
                .SingleOrDefaultAsync(user => user.UserName == loginDto.Username.ToLower());


            if (user == null) return Unauthorized("Invalid username");

            var result = await SignInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

            return new IdentityUserDto
            {
                Username = user.UserName,
                Token = TokenService.CreateToken(user),
            };
        }

        
    }
}