using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blogger.Core.Entities;
using Blogger.Infrastructure;
using Blogger.WebApi.Resources.User;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Blogger.Tests.WebApi
{
    [TestFixture]
    public class UserTests
    {
        [SetUp]
        public void SetUpTests()
        {
            _webApplicationFactory = new CustomWebApplicationFactory();
            _client = _webApplicationFactory.CreateClient();
        }

        private CustomWebApplicationFactory _webApplicationFactory;
        private HttpClient _client;

        [TestCase]
        public async Task Register_InvalidResource_ReturnsBadRequest()
        {
            // Arrange
            var user = new SaveUserResource();

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", user);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase]
        public async Task Register_ValidRequest_CreatesUser()
        {
            // Arrange
            var faker = new Faker();

            var user = new SaveUserResource
            {
                UserName = faker.Person.UserName,
                Email = faker.Person.Email,
                Password = "SecureP455wordEx4mp!e"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", user);

            // Assert
            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(context.Users.Count(), Is.EqualTo(1));
            }
        }

        [TestCase]
        public async Task Login_InvalidResource_ReturnsBadRequest()
        {
            // Arrange
            var login = new LoginUserResource();

            // Act
            var response = await _client.PostAsJsonAsync("/api/users/login", login);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase]
        public async Task Login_BadCredentials_ReturnsUnauthorised()
        {
            // Arrange
            var login = new LoginUserResource
            {
                UserName = "test",
                Password = "invalid"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users/login", login);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task Login_ValidCredentials_SignsIn()
        {
            // Arrange
            var login = new LoginUserResource
            {
                UserName = "test",
                Password = "SecureP455wordEx4mp!e"
            };

            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                await userManager.CreateAsync(new ApplicationUser { UserName = login.UserName, Email = "test@mail.com" }, login.Password);
            }

            // Act
            var response = await _client.PostAsJsonAsync("/api/users/login", login);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<UserResource>(responseString);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseObject.UserName, Is.EqualTo(login.UserName));
            Assert.That(responseObject.Token.Length, Is.GreaterThan(100));
        }

        [TestCase]
        public async Task GetCurrentUser_InvalidAuthenticationHeader_ReturnsUnauthorised()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid_jwt");

            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task GetUser_ValidAuthenticationHeader_ReturnsUser()
        {
            // Arrange
            var user = await SeedData.SignInAndSetAuthorizationHeader(_webApplicationFactory, _client);

            // Act
            var response = await _client.GetAsync("/api/users");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<UserResource>(responseString);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseObject.UserName, Is.EqualTo(user.UserName));
            Assert.That(responseObject.Token.Length, Is.GreaterThan(100));
        }
    }
}