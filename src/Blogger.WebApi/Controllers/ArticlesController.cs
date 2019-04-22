using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ITagRepository _tagRepository;

        public ArticlesController(IMapper mapper, IRepository repository, IArticleRepository articleRepository, ICommentRepository commentRepository, IUserResolverService userResolverService, IFavoriteRepository favoriteRepository, ITagRepository tagRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _userResolverService = userResolverService;
            _favoriteRepository = favoriteRepository;
            _tagRepository = tagRepository;
        }

        [HttpPost]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> CreateArticle([FromBody] SaveArticleResource saveArticleResource)
        {
            // Create article
            var article = _mapper.Map<Article>(saveArticleResource);
            article.CreatedAt = DateTime.Now;
            article.UpdatedAt = DateTime.Now;
            article.AuthorId = _userResolverService.GetUserId();

            await _repository.AddAsync(article);

            // Add tags to article
            var tags = saveArticleResource.TagList.Select(t => t.ToLower()).Distinct();

            foreach (var name in tags)
            {
                var tag = await _tagRepository.GetOrCreateAsync(name);

                var articleTag = new ArticleTag
                {
                    ArticleId = article.Id,
                    TagId = tag.Id
                };

                await _repository.AddAsync(articleTag);
            }

            // Retrieve newly created article
            var entity = await _articleRepository.GetBySlugAsync(article.Slug);
            var result = _mapper.Map<ArticleResource>(entity);

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
            var signedInUserId = _userResolverService.GetUserId();

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

            if (!article.Favorited)
            {
                var favorite = new ArticleFavorite
                {
                    ArticleId = article.Id,
                    ObserverId = _userResolverService.GetUserId()
                };

                await _repository.AddAsync(favorite);

                article.Favorited = true;
            }

            var result = _mapper.Map<ArticleResource>(article);

            return Ok(result);
        }

        [HttpDelete("{slug}/favorite")]
        [Authorize]
        public async Task<IActionResult> DeleteFavorite(string slug)
        {
            var article = await _articleRepository.GetBySlugAsync(slug);

            if (article == null)
            {
                return NotFound();
            }

            if (article.Favorited)
            {
                await _favoriteRepository.RemoveAsync(article.Id, _userResolverService.GetUserId());

                article.Favorited = false;
            }

            var result = _mapper.Map<ArticleResource>(article);

            return Ok(result);
        }
    }
}