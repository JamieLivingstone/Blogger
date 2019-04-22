using System.Threading.Tasks;
using Blogger.Core.Entities;

namespace Blogger.Core.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> GetOrCreate(string tagName);
    }
}