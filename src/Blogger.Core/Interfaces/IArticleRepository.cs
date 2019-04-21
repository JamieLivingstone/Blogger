using System.Collections.Generic;
using System.Threading.Tasks;
using Blogger.Core.Entities;

namespace Blogger.Core.Interfaces
{
    public interface IArticleRepository
    {
        Task<Article> GetBySlugAsync(string slug);
    }
}