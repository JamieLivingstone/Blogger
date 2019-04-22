using System.Collections.Generic;
using System;
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

        public async Task<List<Article>> GetFeedAsync(int? limit, int? offset, string tag, string author, string favorited)
        {
            IQueryable<Article> queryable = _dbContext.Articles
                .Include(a => a.Author)
                .Include(a => a.Tags);

            if (tag != null)
            {
                var tagEntity = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tag.ToLower());

                if (tagEntity != null)
                {
                    queryable = queryable.Where(a => a.Tags.Select(t => t.TagId).Contains(tagEntity.Id));
                }
            }

            if (author != null)
            {
                queryable = queryable.Where(a => string.Equals(a.Author.Id, author, StringComparison.OrdinalIgnoreCase));
            }

            if (favorited != null)
            {
                queryable = queryable.Where(a => a.Favorites.Select(f => f.ObserverId).Contains(favorited));
            }

            return await queryable
                .OrderByDescending(a => a.CreatedAt)
                .Skip(offset ?? 0)
                .Take(limit ?? 20)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}