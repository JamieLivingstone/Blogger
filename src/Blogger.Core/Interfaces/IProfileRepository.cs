using System.Threading.Tasks;
using Blogger.Core.Entities;

namespace Blogger.Core.Interfaces
{
    public interface IProfileRepository
    {
        Task<ApplicationUser> GetByUserNameAsync(string userName);
    }
}