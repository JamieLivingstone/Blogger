using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogger.Core.Entities;
using Blogger.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Comment>> GetAllByArticleIdAsync(int articleId)
        {
            return await _dbContext.Comments
                .Where(comment => comment.ArticleId == articleId)
                .Include(comment => comment.Author)
                .OrderByDescending(comment => comment.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _dbContext.Comments
                .Where(comment => comment.Id == id)
                .Include(comment => comment.Author)
                .FirstOrDefaultAsync();
        }
    }
}