using System;
using System.Threading.Tasks;
using AutoMapper;
using Blogger.Core.Entities;
using Blogger.Core.Interfaces;
using Blogger.WebApi.Filters;
using Blogger.WebApi.Resources.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(
            IMapper mapper, 
            IRepository repository, 
            UserManager<ApplicationUser> userManager,
            IArticleRepository articleRepository
        )
        {
            _mapper = mapper;
            _repository = repository;
            _userManager = userManager;
            _articleRepository = articleRepository;
        }
        
        [HttpPost]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> CreateArticle([FromBody] SaveArticleResource saveArticleResource)
        {
            var now = DateTime.Now;
            
            var article = _mapper.Map<Article>(saveArticleResource);
            article.CreatedAt = now;
            article.UpdatedAt = now;
            article.Author = await _userManager.GetUserAsync(HttpContext.User);

            await _repository.AddAsync(article);

            var result = _mapper.Map<ArticleResource>(article);
           
            return Created($"api/articles/{article.Id}", result);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetArticleBySlug(string slug)
        {
            var article = await _articleRepository.GetBySlugAsync(slug);

            if (article == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<ArticleResource>(article);

            return Ok(result);
        }
    }
}