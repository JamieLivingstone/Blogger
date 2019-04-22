using System.Threading.Tasks;

namespace Blogger.Core.Interfaces
{
    public interface IFollowerRepository
    {
        Task RemoveAsync(string targetId, string observerId);
    }
}