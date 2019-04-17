using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Blogger.Infrastructure;
using Blogger.WebApi.Resources.Article;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Blogger.Tests.Integration.Api
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
                Body = _faker.Lorem.Sentences()
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
            var articles = await SeedData.SeedArticlesAsync(_webApplicationFactory, 3);
            var articleToRetrieve = articles[1];
            
            // Act
            var response = await _client.GetAsync($"/api/articles/{articleToRetrieve.Slug}");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ArticleResource>(responseString);
        
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseObject.Slug, Is.EqualTo(articleToRetrieve.Slug));
            Assert.That(responseObject.Body, Is.EqualTo(articleToRetrieve.Body));
            Assert.That(responseObject.Author.UserName, Is.EqualTo(articleToRetrieve.Author.UserName));
        }
    }
}