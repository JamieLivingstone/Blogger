using System.Collections.Generic;
using System.Threading.Tasks;
using Blogger.Core.Entities;

namespace Blogger.Core.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllByArticleIdAsync(int articleId);
        Task<Comment> GetByIdAsync(int id);
    }
}