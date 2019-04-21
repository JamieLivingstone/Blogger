using System.Collections.Generic;
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

        public ArticleRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Article> GetBySlugAsync(string slug)
        {
            return await _dbContext.Articles
                .Where(article => article.Slug == slug)
                .Include(article => article.Author)
                .FirstOrDefaultAsync();
        }
    }
}