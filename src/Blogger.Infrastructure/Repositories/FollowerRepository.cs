using System.Threading.Tasks;
using Blogger.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Infrastructure.Repositories
{
    public class FollowerRepository : IFollowerRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FollowerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<bool> IsFollowing(string targetId, string observerId)
        {
            var following = await _dbContext.Followers.FirstOrDefaultAsync(f => f.TargetId == targetId && f.ObserverId == observerId);

            return following != null;
        }

        public async Task RemoveAsync(string targetId, string observerId)
        {
            var following = await _dbContext.Followers.FirstOrDefaultAsync(f => f.TargetId == targetId && f.ObserverId == observerId);

            if (following != null)
            {
                _dbContext.Remove(following);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}