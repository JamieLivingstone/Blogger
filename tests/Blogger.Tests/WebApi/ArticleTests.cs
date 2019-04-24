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
            await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
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
            var user = await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);

            var faker = new Faker();

            var article = new SaveArticleResource
            {
                Title = faker.Lorem.Word(),
                Description = faker.Lorem.Sentence(),
                Body = faker.Lorem.Sentences(),
                TagList = new List<string> { "gaming", "gaming", "music" }
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
                Assert.That(responseObject.Author.UserName, Is.EqualTo(user.UserName));
                CollectionAssert.AreEquivalent(responseObject.TagList.ToList(), new List<string> { "gaming", "music" });

                Assert.That(dbContext.Articles.Count(), Is.EqualTo(1));
                Assert.That(dbContext.Tags.Count(), Is.EqualTo(2));
                Assert.That(dbContext.ArticleTags.Count(), Is.EqualTo(2));
            }
        }

        [TestCase]
        public async Task GetFeed_ArticlesExist_AppliesLimitAndOffset()
        {
            // Arrange
            var articles = await SeedData.SeedArticlesAsync(_webApplicationFactory, 5);

            // Act
            var result = await _client.GetAsync("/api/articles/feed?limit=2&offset=0");
            var resultString = await result.Content.ReadAsStringAsync();
            var resultArray = JsonConvert.DeserializeObject<List<ArticleResource>>(resultString);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resultArray.Count(), Is.EqualTo(2));
        }

        [TestCase]
        public async Task GetArticle_DoesNotExist_ReturnsNotFound()
        {
            // Arrange
            const string slug = "does-not-exist";

            // Act
            var response = await _client.GetAsync($"/api/articles/{slug}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task GetArticle_Exists_ReturnsArticle()
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
        public async Task DeleteArticle_NotSignedIn_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.DeleteAsync("/api/articles/fake-article-slug");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task DeleteArticle_DoesNotExist_ReturnsNotFound()
        {
            // Arrange
            await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);

            // Act
            var response = await _client.DeleteAsync("/api/articles/fake-article-slug");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task DeleteArticle_DoesNotHavePermissionsToDelete_ReturnsUnauthorized()
        {
            // Arrange
            await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory);

            // Act
            var response = await _client.DeleteAsync($"/api/articles/{article.Slug}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task DeleteArticle_ValidRequest_DeletesArticle()
        {
            // Arrange
            var user = await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);
            var article = await SeedData.SeedArticleAsync(_webApplicationFactory, user);

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
    }
}