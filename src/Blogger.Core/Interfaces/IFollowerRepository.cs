using System.Threading.Tasks;

namespace Blogger.Core.Interfaces
{
    public interface IFollowerRepository
    {
        Task<bool> IsFollowing(string targetId, string observerId);
        Task RemoveAsync(string targetId, string observerId);
    }
}