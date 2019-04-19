using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Blogger.Core.Entities;
using Blogger.Core.Interfaces;
using Blogger.WebApi.Filters;
using Blogger.WebApi.Resources.Article;
using Blogger.WebApi.Resources.Comment;
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
        
        [HttpDelete("{slug}")]
        [Authorize]
        public async Task<IActionResult> DeleteArticleBySlug(string slug)
        {
            var signedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var article = await _articleRepository.GetBySlugAsync(slug);

            if (article == null)
            {
                return NotFound();
            }

            if (article.Author.Id != signedInUserId)
            {
                return Unauthorized("You do not have permissions to delete this article!");
            }

            await _repository.DeleteAsync(article);

            var result = _mapper.Map<ArticleResource>(article);

            return Ok(result);
        }

        [HttpPost("{slug}/comments")]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> AddCommentToArticle(string slug, [FromBody] SaveCommentResource saveCommentResource)
        {
            var article = await _articleRepository.GetBySlugAsync(slug);

            if (article == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                Body = saveCommentResource.Body,
                ArticleId = article.Id,
                Author = await _userManager.GetUserAsync(HttpContext.User),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repository.AddAsync(comment);

            var result = _mapper.Map<CommentResource>(comment);
            
            return Created($"/api/articles/{article.Slug}/comments/{comment.Id}", result);
        }

        [HttpGet("{slug}/comments")]
        public async Task<IActionResult> GetComments(string slug)
        {
            var article = await _articleRepository.GetBySlugAsync(slug);

            if (article == null)
            {
                return NotFound();
            }

            var comments = await _articleRepository.GetCommentsByArticleIdAsync(article.Id);

            var result = _mapper.Map<IList<CommentResource>>(comments);
            
            return Ok(result);
        }
    }
}