using System.Threading.Tasks;
using Blogger.WebApi.Filters;
using Blogger.WebApi.Resources.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/articles")]
    public class ArticlesController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> CreateArticle([FromBody] SaveArticleResource articleResource)
        {
            return Ok();
        }
    }
}