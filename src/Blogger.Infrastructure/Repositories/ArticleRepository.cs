using System.Linq;
using System.Threading.Tasks;
using Blogger.Core.Entities;
using Blogger.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Infrastructure.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserResolverService _userResolverService;

        public ArticleRepository(ApplicationDbContext dbContext, IUserResolverService userResolverService)
        {
            _dbContext = dbContext;
            _userResolverService = userResolverService;
        }

        public async Task<Article> GetBySlugAsync(string slug)
        {
            var article = await _dbContext.Articles
                .Where(a => a.Slug == slug)
                .Include(a => a.Author)
                .Include(a => a.Tags)
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return null;
            }

            var signedInUserId = _userResolverService.GetUserId();

            if (signedInUserId != null)
            {
                var favorited = await _dbContext.Favorites.FirstOrDefaultAsync(f => f.ArticleId == article.Id && f.ObserverId == signedInUserId);

                if (favorited != null)
                {
                    article.Favorited = true;
                }
            }

            article.FavoritesCount = _dbContext.Favorites.Count(f => f.ArticleId == article.Id);

            return article;
        }
    }
}