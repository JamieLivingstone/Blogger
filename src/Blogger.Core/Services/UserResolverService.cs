using System.Security.Claims;
using System.Threading.Tasks;
using Blogger.Core.Entities;
using Blogger.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Blogger.Core.Services
{
    public class UserResolverService : IUserResolverService
    {
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserResolverService(IHttpContextAccessor context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetUserAsync()
        {
            return await _userManager.GetUserAsync(_context.HttpContext.User);
        }

        public string GetUserId()
        {
            return _context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}