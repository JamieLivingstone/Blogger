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

namespace Blogger.Tests
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

        public static async Task<Article> SeedArticleAsync(CustomWebApplicationFactory factory, ApplicationUser author = null)
        {
            if (author == null)
            {
                var seededUsers = await SeedData.SeedUsersAsync(factory, 1);
                author = seededUsers[0];
            }

            // Seed article
            var faker = new Faker();
            var title = faker.Lorem.Sentence();

            var article = new Article
            {
                Slug = title.GenerateSlug(),
                Title = title,
                Description = faker.Lorem.Sentence(),
                Body = faker.Lorem.Sentences(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorId = author.Id
            };

            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.AddAsync(article);
                await dbContext.SaveChangesAsync();
            }

            // Seed comments
            var comments = new List<Comment>();

            for (var i = 0; i < 10; i++)
            {
                comments.Add(new Comment
                {
                    Body = faker.Lorem.Sentence(),
                    ArticleId = article.Id,
                    AuthorId = author.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.AddRangeAsync(comments);
                await dbContext.SaveChangesAsync();

                article.Comments = comments.ToList();
            }

            // Seed tags
            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var tags = new List<Tag> {
                    new Tag("foo"),
                    new Tag("bar")
                };

                await dbContext.Tags.AddRangeAsync(tags);
                await dbContext.SaveChangesAsync();

                var articleTags = new List<ArticleTag> {
                    new ArticleTag { TagId = tags[0].Id, ArticleId = article.Id },
                    new ArticleTag { TagId = tags[1].Id, ArticleId = article.Id }
                };

                await dbContext.ArticleTags.AddRangeAsync(articleTags);
                await dbContext.SaveChangesAsync();

                article.Tags = articleTags;
            }

            // Return seeded article
            return article;
        }

        public async static Task<List<Tag>> SeedTagsAsync(CustomWebApplicationFactory factory)
        {
            var tags = new List<Tag> {
                new Tag("gaming"),
                new Tag("music"),
                new Tag("tech"),
                new Tag("news")
            };

            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.AddRangeAsync(tags);
                await dbContext.SaveChangesAsync();
            }

            return tags;
        }
    }
}