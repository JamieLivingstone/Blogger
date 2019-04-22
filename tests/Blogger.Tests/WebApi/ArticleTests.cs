using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Blogger.Infrastructure;
using Blogger.WebApi.Resources.Article;
using Blogger.WebApi.Resources.Comment;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Blogger.Tests.WebApi
{
    [TestFixture]
    public class ArticleTests
    {
        private CustomWebApplicationFactory _webApplicationFactory;
        private HttpClient _client;
        private readonly Faker _faker = new Faker();

        [SetUp]
        public void SetUpTests()
        {
            _webApplicationFactory = new CustomWebApplicationFactory();
            _client = _webApplicationFactory.CreateClient();
        }

        [TestCase]
        public async Task CreateArticle_NotSignedIn_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.PostAsync("/api/articles", null);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task CreateArticle_InvalidResource_ReturnsBadRequest()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = new SaveArticleResource();

            // Act
            var response = await _client.PostAsJsonAsync("/api/articles", article);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase]
        public async Task CreateArticle_ValidResource_CreatesArticle()
        {
            // Arrange
            var signedInUser = await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);

            var article = new SaveArticleResource
            {
                Title = _faker.Lorem.Word(),
                Description = _faker.Lorem.Sentence(),
                Body = _faker.Lorem.Sentences(),
                TagList = new List<string> { "gaming", "gaming", "music" } // Duplicate category (to assert tag logic handles duplicates)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/articles", article);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ArticleResource>(responseString);

            // Assert

            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(responseObject.Title, Is.EqualTo(article.Title));
                Assert.That(responseObject.Description, Is.EqualTo(article.Description));
                Assert.That(responseObject.Body, Is.EqualTo(article.Body));
                Assert.That(responseObject.Author.UserName, Is.EqualTo(signedInUser.UserName));
                Assert.That(dbContext.Articles.Count(), Is.EqualTo(1));
            }
        }

        [TestCase]
        public async Task GetArticleBySlug_DoesNotExist_ReturnsNotFound()
        {
            // Arrange
            const string slug = "does-not-exist";

            // Act
            var response = await _client.GetAsync($"/api/articles/{slug}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task GetArticleBySlug_ArticleExists_ReturnsArticle()
        {
            // Arrange
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, null);

            // Act
            var response = await _client.GetAsync($"/api/articles/{article.Slug}");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ArticleResource>(responseString);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseObject.Slug, Is.EqualTo(article.Slug));
            Assert.That(responseObject.Body, Is.EqualTo(article.Body));
        }

        [TestCase]
        public async Task DeleteArticleBySlug_NotSignedIn_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.DeleteAsync("/api/articles/fake-article-slug");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task DeleteArticleBySlug_ArticleDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);

            // Act
            var response = await _client.DeleteAsync("/api/articles/fake-article-slug");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task DeleteArticleBySlug_ArticleExistsButUserIsNotTheAuthor_ReturnsUnauthorized()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, null);

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task DeleteArticleBySlug_ArticleExistsAndUserIsTheAuthor_DeletesArticleAndReturnsOk()
        {
            // Arrange
            var signedInUser = await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, signedInUser);

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}");

            // Assert
            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(dbContext.Articles.Count(), Is.EqualTo(0));
            }
        }

        [TestCase]
        public async Task AddCommentToArticle_NotSignedIn_ReturnsUnauthorized()
        {
            // Arrange
            var comment = new SaveCommentResource();
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, null);

            // Act
            var response = await _client.PostAsJsonAsync($"/api/articles/{article.Slug}/comments", comment);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task AddCommentToArticle_ArticleDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var comment = new SaveCommentResource { Body = "Test comment!" };
            const string slug = "i-do-not-exist";

            // Act
            var response = await _client.PostAsJsonAsync($"/api/articles/{slug}/comments", comment);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task AddCommentToArticle_ArticleExists_AddsCommentAndReturnsComment()
        {
            // Arrange
            var signedInUser = await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var comment = new SaveCommentResource { Body = "Test comment!" };
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, signedInUser);

            // Act
            var response = await _client.PostAsJsonAsync($"/api/articles/{article.Slug}/comments", comment);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<CommentResource>(responseString);

            // Assert
            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(responseObject.Body, Is.EqualTo(comment.Body));
                Assert.That(responseObject.Author.UserName, Is.EqualTo(signedInUser.UserName));
                Assert.That(dbContext.Comments.Count(), Is.EqualTo(article.Comments.Count() + 1));
            }
        }

        [TestCase]
        public async Task GetComments_ArticleDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            const string slug = "i-do-not-exist";

            // Act
            var response = await _client.GetAsync($"/api/articles/{slug}/comments");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task GetComments_ArticleExistsWithComments_ReturnsComments()
        {
            // Arrange
            var signedInUser = await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, signedInUser);

            // Act
            var response = await _client.GetAsync($"/api/articles/{article.Slug}/comments");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseArray = JsonConvert.DeserializeObject<List<CommentResource>>(responseString);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseArray.Count, Is.EqualTo(article.Comments.Count));
            Assert.That(responseArray[0].Author.UserName, Is.EqualTo(signedInUser.UserName));
        }

        [TestCase]
        public async Task DeleteComment_NotSignedIn_ReturnsUnauthorized()
        {
            // Slug
            const string slug = "does-not-exist";
            const int commentId = 1;

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{slug}/comments/{commentId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task DeleteComment_ArticleDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);

            // Act
            var response = await _client.DeleteAsync("/api/articles/does-not-exist/comments/1");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task DeleteComment_CommentDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var signedInUser = await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, signedInUser);

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}/comments/999");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task DeleteComment_SignedInUserDidNotCreateTheComment_ReturnsUnauthorized()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, null);

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}/comments/{article.Comments[0].Id}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task DeleteComment_CommentExistsAndWasCreatedBySignedInUser_DeletesComment()
        {
            // Arrange
            var signedInUser = await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, signedInUser);
            var commentId = article.Comments[0].Id;

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}/comments/{commentId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [TestCase]
        public async Task Favorite_NotSignedIn_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.PostAsync("/api/articles/article-name/favorite", null);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task Favorite_ArticleDoesNotExist_ReturnsNotFound()
        {
            // Arrange 
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);

            // Act
            var response = await _client.PostAsync("/api/articles/does-not-exist/favorite", null);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task Favorite_ArticleExistsAndUserSignedIn_FavoritesArticle()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, null);

            // Act
            var response = await _client.PostAsync($"/api/articles/{article.Slug}/favorite", null);

            // Assert
            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(dbContext.Favorites.Count(), Is.EqualTo(1));
            }
        }

        [TestCase]
        public async Task RemoveFavorite_NotSignedIn_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.DeleteAsync("/api/articles/article-name/favorite");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task RemoveFavorite_ArticleDoesNotExist_ReturnsNotFound()
        {
            // Arrange 
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);

            // Act
            var response = await _client.DeleteAsync("/api/articles/does-not-exist/favorite");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task RemoveFavorite_FavoritedArticle_RemovesFavorite()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, null);

            // Act
            await _client.PostAsync($"/api/articles/{article.Slug}/favorite", null);
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}/favorite");

            // Assert
            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(dbContext.Favorites.Count(), Is.EqualTo(0));
            }
        }
    }
}