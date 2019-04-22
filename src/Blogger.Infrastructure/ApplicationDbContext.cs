using Blogger.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Follower> Followers { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ArticleFavorite> Favorites { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Followers
            modelBuilder.Entity<Follower>(builder =>
            {
                builder.HasKey(favorite => new { favorite.TargetId, favorite.ObserverId });
            });

            // Articles
            modelBuilder.Entity<Article>(builder =>
            {
                builder
                    .HasMany(article => article.Comments)
                    .WithOne(comment => comment.Article)
                    .HasForeignKey(comment => comment.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder
                    .HasOne(article => article.Author)
                    .WithMany(author => author.Articles)
                    .HasForeignKey(article => article.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Favorites
            modelBuilder.Entity<ArticleFavorite>(builder =>
            {
                builder
                    .HasOne(favorite => favorite.Article)
                    .WithMany(article => article.Favorites)
                    .HasForeignKey(favorite => favorite.ArticleId);

                builder
                    .HasOne(favorite => favorite.Observer)
                    .WithMany(observer => observer.Favorites)
                    .HasForeignKey(favorite => favorite.ObserverId);
            });

            // Tags
            modelBuilder.Entity<ArticleTag>(builder =>
            {
                builder.HasOne(articleTag => articleTag.Article)
                    .WithMany(article => article.Tags)
                    .HasForeignKey(articleTag => articleTag.ArticleId);

                builder.HasOne(articleTag => articleTag.Tag)
                    .WithMany(tag => tag.ArticleTags)
                    .HasForeignKey(articleTag => articleTag.TagId);
            });
        }
    }
}