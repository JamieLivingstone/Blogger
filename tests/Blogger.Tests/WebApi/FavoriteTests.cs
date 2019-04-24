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
    public class FavoriteTests
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
        public async Task CreateFavorite_NotSignedIn_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.PostAsync("/api/articles/article-name/favorite", null);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task CreateFavorite_ArticleDoesNotExist_ReturnsNotFound()
        {
            // Arrange 
            await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);

            // Act
            var response = await _client.PostAsync("/api/articles/does-not-exist/favorite", null);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task CreateFavorite_ValidRequest_FavoritesArticle()
        {
            // Arrange
            var user = await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory);

            // Act
            var response = await _client.PostAsync($"/api/articles/{article.Slug}/favorite", null);

            // Assert
            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var favorite = dbContext.Favorites.First();

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(dbContext.Favorites.Count(), Is.EqualTo(1));
                Assert.That(favorite.ArticleId, Is.EqualTo(article.Id));
                Assert.That(favorite.ObserverId, Is.EqualTo(user.Id));
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
            await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);

            // Act
            var response = await _client.DeleteAsync("/api/articles/does-not-exist/favorite");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task RemoveFavorite_ValidRequest_RemovesFavorite()
        {
            // Arrange
            await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
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