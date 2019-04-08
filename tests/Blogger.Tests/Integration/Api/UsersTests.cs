using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blogger.Core.Entities;
using Blogger.Infrastructure;
using Blogger.WebApi.Resources.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Blogger.Tests.Integration.Api
{
    [TestFixture]
    public class UsersTests
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
        public async Task Register_GivenAnInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var user = new SaveUserResource();

            // Act
            var result = await _client.PostAsJsonAsync("/api/users", user);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase]
        public async Task Register_GivenAValidModel_CreatesUser()
        {
            // Arrange
            var user = new SaveUserResource
            {
                UserName = "test",
                Email = "example@mail.com",
                Password = "SecureP455wordEx4mp!e"
            };

            // Act
            var result = await _client.PostAsJsonAsync("/api/users", user);

            // Assert
            using (var scope = _webApplicationFactory.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(context.Users.Count(), Is.EqualTo(1));
            }
        }

        [TestCase]
        public async Task Login_GivenAnInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var login = new LoginUserResource();

            // Act
            var result = await _client.PostAsJsonAsync("/api/users/login", login);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase]
        public async Task Login_GivenInvalidCredentials_ReturnsUnauthorised()
        {
            // Arrange
            var login = new LoginUserResource
            {
                UserName = "test",
                Password = "invalid"
            };

            // Act
            var result = await _client.PostAsJsonAsync("/api/users/login", login);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase]
        public async Task Login_GivenValidCredentials_ReturnsUserResource()
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
            var result = await _client.PostAsJsonAsync("/api/users/login", login);
            
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [TestCase]
        public async Task GetCurrentUser_InvalidJWT_ReturnsUnauthorised()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid_jwt");

            // Act
            var result = await _client.GetAsync("/api/users");

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}