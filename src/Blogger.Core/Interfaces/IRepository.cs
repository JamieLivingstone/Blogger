using System.Threading.Tasks;
using Blogger.Core.Entities;

namespace Blogger.Core.Interfaces
{
    public interface IRepository
    {
        Task<T> AddAsync<T>(T entity) where T : BaseEntity;
        Task DeleteAsync<T>(T entity) where T : BaseEntity;
    }
}