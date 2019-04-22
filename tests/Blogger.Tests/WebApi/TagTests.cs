using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Blogger.Tests.WebApi
{
    [TestFixture]
    public class TagTests
    {
        private CustomWebApplicationFactory _webApplicationFactory;
        private System.Net.Http.HttpClient _client;

        [SetUp]
        public void SetUpTests()
        {
            _webApplicationFactory = new CustomWebApplicationFactory();
            _client = _webApplicationFactory.CreateClient();
        }

        [TestCase]
        public async Task GetTags_NoTags_ReturnsEmptyList()
        {
            // Act
            var result = await _client.GetAsync("/api/tags");
            var resultString = await result.Content.ReadAsStringAsync();
            var resultArray = JsonConvert.DeserializeObject<List<string>>(resultString);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resultArray.Count(), Is.EqualTo(0));
        }

        [TestCase]
        public async Task GetTags_TagsInDatabase_ReturnsTags()
        {
            // Arrange
            var seededTags = await SeedData.SeedTagsAsync(_webApplicationFactory);

            // Act
            var result = await _client.GetAsync("/api/tags");
            var resultString = await result.Content.ReadAsStringAsync();
            var resultArray = JsonConvert.DeserializeObject<List<string>>(resultString);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resultArray.Count(), Is.EqualTo(seededTags.Count()));
        }
    }
}