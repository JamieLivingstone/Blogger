using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Blogger.WebApi.Resources.Article;
using NUnit.Framework;

namespace Blogger.Tests.Integration.Api
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
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var article = new SaveArticleResource();
            
            // Act
            var response = await _client.PostAsJsonAsync("/api/articles", article);
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}