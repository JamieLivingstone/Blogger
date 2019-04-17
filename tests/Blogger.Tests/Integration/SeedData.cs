using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blogger.Core.Entities;
using Blogger.Infrastructure.Security;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Blogger.Tests.Integration
{
    public static class SeedData
    {
        public static async Task<List<ApplicationUser>> SeedUsersAsync(CustomWebApplicationFactory factory, int usersToCreate = 5)
        {
            var users = new ApplicationUser[usersToCreate];
            var faker = new Faker();
            
            for (var i = 0; i < users.Length; i++)
            {
                users[i] = new ApplicationUser
                {
                    UserName = faker.Person.UserName,
                    Email = faker.Person.Email,
                    Bio = faker.Lorem.Sentence(),
                    Image = faker.Person.Avatar
                };
            }
            
            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "SomeCompl3xP455word@123");
                }
            }

            return users.ToList();
        }

        public static async Task<ApplicationUser> SeedUserAndMutateAuthorizationHeader(CustomWebApplicationFactory factory, HttpClient httpClient)
        {
            var user = new ApplicationUser
            {
                UserName = "test",
                Email = "test@mail.com"
            };
            
            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var jwtTokenGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
                
                await userManager.CreateAsync(user, "SomeCompl3xP455word@123");

                var token = jwtTokenGenerator.CreateToken(user);
                user.Token = token;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return user;
        }
    }
}