using System.Threading.Tasks;

namespace Blogger.Core.Interfaces
{
    public interface IFavoriteRepository
    {
        Task RemoveAsync(int articleId, string observerId);
    }
}