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
                .FirstOrDefaultAsync();

            var signedInUserId = _userResolverService.GetUserId();

            if (article != null && signedInUserId != null)
            {
                var favorited = await _dbContext.Favorites.FirstOrDefaultAsync(f =>f.ArticleId == article.Id && f.ObserverId == signedInUserId);

                if (favorited != null)
                {
                    article.Favorited = true;                    
                }
            }

            return article;
        }
    }
}