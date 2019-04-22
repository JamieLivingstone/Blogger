using System.Linq;
using System.Threading.Tasks;
using Blogger.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagRepository.GetTagsAsync();
            var result = tags.Select(tag => tag.Id);

            return Ok(result);
        }
    }
}