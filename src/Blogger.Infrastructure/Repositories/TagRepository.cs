using System.Threading.Tasks;
using Blogger.Core.Entities;
using Blogger.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TagRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tag> GetOrCreate(string tagName)
        {
            tagName = tagName.ToLower();

            var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.TagId == tagName);

            if (tag == null)
            {
                tag = new Tag(tagName);
                await _dbContext.AddAsync(tag);
                await _dbContext.SaveChangesAsync();
            }

            return tag;
        }
    }
}