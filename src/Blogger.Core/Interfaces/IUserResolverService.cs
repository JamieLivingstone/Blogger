using System.Threading.Tasks;
using Blogger.Core.Entities;

namespace Blogger.Core.Interfaces
{
    public interface IUserResolverService
    {
        Task<ApplicationUser> GetUserAsync();
        string GetUserId();
    }
}