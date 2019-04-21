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
        private readonly IArticleRepository _articleRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserResolverService _userResolverService;

        public ArticlesController(
            IMapper mapper, 
            IRepository repository, 
            IArticleRepository articleRepository,
            ICommentRepository commentRepository,
            IUserResolverService userResolverService
        )
        {
            _mapper = mapper;
            _repository = repository;
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _userResolverService = userResolverService;
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
            article.Author = await _userResolverService.GetUserAsync();

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
            var article = await _articleRepository.GetBySlugAsync(slug);

            if (article == null)
            {
                return NotFound();
            }

            if (article.Author.Id != _userResolverService.GetUserId())
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
                Author = await _userResolverService.GetUserAsync(),
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

            var comments = await _commentRepository.GetAllByArticleIdAsync(article.Id);

            var result = _mapper.Map<IList<CommentResource>>(comments);
            
            return Ok(result);
        }

        [HttpDelete("{slug}/comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(string slug, int commentId)
        {
            var article = await _articleRepository.GetBySlugAsync(slug);
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (article == null || comment == null)
            {
                return NotFound();
            }

            if (comment.Author.Id != _userResolverService.GetUserId())
            {
                return Unauthorized();
            }
            
            await _repository.DeleteAsync(comment);

            var result = _mapper.Map<CommentResource>(comment);

            return Ok(result);
        }

        [HttpPost("{slug}/favorite")]
        [Authorize]
        public async Task<IActionResult> Favorite(string slug)
        {
            var article = await _articleRepository.GetBySlugAsync(slug);

            if (article == null)
            {
                return NotFound();
            }
            
            return Ok();
        }
    }
}