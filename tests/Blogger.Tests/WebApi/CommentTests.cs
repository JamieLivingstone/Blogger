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
    public class CommentTests
    {
        private CustomWebApplicationFactory _webApplicationFactory;
        private HttpClient _client;

        [SetUp]
        public void SetUpTests()
        {
            _webApplicationFactory = new CustomWebApplicationFactory();
            _client = _webApplicationFactory.CreateClient();
        }

        [TestCase]
        public async Task CreateComment_NotSignedIn_ReturnsUnauthorized()
        {
            // Arrange
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory);

            // Act
            var response = await _client.PostAsJsonAsync($"/api/articles/{article.Slug}/comments", new SaveCommentResource());

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task CreateComment_ArticleDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
            var comment = new SaveCommentResource { Body = "Test comment!" };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/articles/article-does-not-exist/comments", comment);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task CreateComment_ArticleExists_AddsComment()
        {
            // Arrange
            var user = await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
            var comment = new SaveCommentResource { Body = "Test comment!" };
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, user);

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
                Assert.That(responseObject.Author.UserName, Is.EqualTo(user.UserName));
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
        public async Task GetComments_ArticleExists_ReturnsComments()
        {
            // Arrange
            var user = await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, user);

            // Act
            var response = await _client.GetAsync($"/api/articles/{article.Slug}/comments");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseArray = JsonConvert.DeserializeObject<List<CommentResource>>(responseString);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseArray.Count, Is.EqualTo(article.Comments.Count));
            Assert.That(responseArray[0].Author.UserName, Is.EqualTo(user.UserName));
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
            await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);

            // Act
            var response = await _client.DeleteAsync("/api/articles/does-not-exist/comments/1");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task DeleteComment_CommentDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var user = await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, user);

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}/comments/999");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task DeleteComment_DoesNotHavePermissionsToDelete_ReturnsUnauthorized()
        {
            // Arrange
            await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory);

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}/comments/{article.Comments[0].Id}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task DeleteComment_ValidRequest_DeletesComment()
        {
            // Arrange
            var user = await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, user);
            var commentId = article.Comments[0].Id;

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}/comments/{commentId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}