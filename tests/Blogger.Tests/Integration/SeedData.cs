using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blogger.Core.Entities;
using Blogger.Infrastructure;
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
            var faker = new Faker();
            var users = new ApplicationUser[usersToCreate];

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

        public static async Task<List<Article>> SeedArticlesAsync(CustomWebApplicationFactory factory,  int articlesToCreate = 5)
        {
            var faker = new Faker();
            var articles = new Article[articlesToCreate];
            var users = await SeedUsersAsync(factory, 3);

            for (var i = 0; i < articles.Length; i++)
            {
                var title = faker.Lorem.Sentence(4);
                
                articles[i] = new Article
                {
                    Slug = title.GenerateSlug(),
                    Title = title,
                    Description = faker.Lorem.Sentence(),
                    Body = faker.Lorem.Sentences(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Author = users[2]
                };
            }
            
            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Articles.AddRangeAsync(articles);
                await dbContext.SaveChangesAsync();
            }

            return articles.ToList();
        }

        public static async Task<Article> SeedArticleWithCommentsAsync(CustomWebApplicationFactory factory, string authorId, int commentsToCreate)
        {
            var faker = new Faker();
            var seededArticles = await SeedArticlesAsync(factory, 1);
            var article = seededArticles[0];
            var comments = new Comment[commentsToCreate];

            for (var i = 0; i < comments.Length; i++)
            {
                comments[i] = new Comment
                {
                    Body = faker.Lorem.Sentence(),
                    ArticleId = article.Id,
                    AuthorId = authorId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
            }
            
            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Comments.AddRangeAsync(comments);
                await dbContext.SaveChangesAsync();
            }
            
            article.Comments = comments.ToList();

            return article;
        }
    }
}