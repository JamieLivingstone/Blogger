using System.Threading.Tasks;
using Blogger.Core.Entities;
using Blogger.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserResolverService _userResolverService;

        public ProfileRepository(ApplicationDbContext dbContext, IUserResolverService userResolverService)
        {
            _dbContext = dbContext;
            _userResolverService = userResolverService;
        }

        public async Task<ApplicationUser> GetByUserNameAsync(string userName)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }

            var signedInUserId = _userResolverService.GetUserId();

            if (signedInUserId != null && signedInUserId != user.Id)
            {
                var isFollowing = await _dbContext.Followers.FirstOrDefaultAsync(follower => follower.TargetId == user.Id && follower.ObserverId == signedInUserId);

                if (isFollowing != null)
                {
                    user.Following = true;
                }
            }

            return user;
        }
    }
}