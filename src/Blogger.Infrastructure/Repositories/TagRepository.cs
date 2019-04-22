using System.Collections.Generic;
using System.Linq;
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

        public async Task<Tag> GetOrCreateAsync(string tagName)
        {
            tagName = tagName.ToLower();

            var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tagName);

            if (tag == null)
            {
                tag = new Tag(tagName);
                await _dbContext.AddAsync(tag);
                await _dbContext.SaveChangesAsync();
            }

            return tag;
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _dbContext.Tags
                .OrderBy(tag => tag.Id)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}