using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Blogger.Infrastructure;
using Blogger.WebApi.Resources.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Blogger.Tests.Integration.Api
{
    [TestFixture]
    public class ProfileTests
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
        public async Task GetProfileByUsername_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            const string userName = "foo";

            // Act
            var response = await _client.GetAsync($"api/profiles/{userName}");
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
        
        [TestCase]
        public async Task GetProfileByUsername_UserExists_ReturnsProfileResource()
        {
            // Arrange
            var seed = await SeedData.SeedUsersAsync(_webApplicationFactory, 10);
            var seededUser = seed[5];

            // Act
            var response = await _client.GetAsync($"api/profiles/{seededUser.UserName}");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ProfileResource>(responseString);
            
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseObject.UserName, Is.EqualTo(seededUser.UserName));
            Assert.That(responseObject.Following, Is.EqualTo(false));
        }

        [TestCase]
        public async Task FollowUser_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);        
            const string userName = "foo";

            // Act
            var result = await _client.PostAsync($"api/profiles/{userName}/follow",  null);
            
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase]
        public async Task FollowUser_UserExists_FollowsUserAndReturnsProfile()
        {
            // Arrange
            var signedInUser = await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var seed = await SeedData.SeedUsersAsync(_webApplicationFactory, 1);
            var userToFollow = seed[0];
            
            // Act
            var response = await _client.PostAsync($"api/profiles/{userToFollow.UserName}/follow",  null);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ProfileResource>(responseString);
            
            // Assert
            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var followerEntity = await dbContext.Followers.FirstOrDefaultAsync();

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(responseObject.UserName, Is.EqualTo(userToFollow.UserName));
                Assert.That(responseObject.Following, Is.EqualTo(true));
                Assert.That(dbContext.Followers.Count(), Is.EqualTo(1));
                Assert.That(followerEntity.TargetId, Is.EqualTo(userToFollow.Id));
                Assert.That(followerEntity.ObserverId, Is.EqualTo(signedInUser.Id));
            }
        }

        [TestCase]
        public async Task UnfollowUser_IsAlreadyFollowing_RemovesFollowAndReturnsProfileResource()
        {
            // Arrange
            await SeedData.SeedUserAndMutateAuthorizationHeader(_webApplicationFactory, _client);
            var seed = await SeedData.SeedUsersAsync(_webApplicationFactory, 1);
            var target = seed[0];
            
            // Act
            await _client.PostAsync($"api/profiles/{target.UserName}/follow", null);
            
            var response = await _client.DeleteAsync($"api/profiles/{target.UserName}/follow");
        
            // Assert
            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(context.Followers.Count(), Is.EqualTo(0));
            }
        }
    }
}