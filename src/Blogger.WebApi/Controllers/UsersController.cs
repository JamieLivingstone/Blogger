using System.Threading.Tasks;
using AutoMapper;
using Blogger.Core.Entities;
using Blogger.Core.Interfaces;
using Blogger.Infrastructure.Security;
using Blogger.WebApi.Filters;
using Blogger.WebApi.Resources.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogger.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserResolverService _userResolverService;

        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper, IJwtTokenGenerator jwtTokenGenerator, IUserResolverService userResolverService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userResolverService = userResolverService;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] SaveUserResource saveUserResource)
        {
            var user = _mapper.Map<ApplicationUser>(saveUserResource);

            var result = await _userManager.CreateAsync(user, saveUserResource.Password);

            if (result.Succeeded)
            {
                return Created($"/api/users/{user.Id}", FormatUser(user));
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] LoginUserResource loginUserResource)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUserResource.UserName, loginUserResource.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.Users.FirstAsync(u => u.UserName == loginUserResource.UserName);

                return Ok(FormatUser(user));
            }

            return Unauthorized();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userResolverService.GetUserAsync();

            return Ok(FormatUser(user));
        }

        private UserResource FormatUser(ApplicationUser applicationUser)
        {
            var user = _mapper.Map<UserResource>(applicationUser);

            user.Token = _jwtTokenGenerator.CreateToken(applicationUser);

            return user;
        }
    }
}