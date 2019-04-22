using System.Threading.Tasks;
using Blogger.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Infrastructure.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FavoriteRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RemoveAsync(int articleId, string observerId)
        {
            var favorite = await _dbContext.Favorites.FirstAsync(f => f.ArticleId == articleId && f.ObserverId == observerId);
            _dbContext.Remove(favorite);
            await _dbContext.SaveChangesAsync();
        }
    }
}