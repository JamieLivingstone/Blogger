using Blogger.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Follower> Followers { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ArticleFavorite> Favorites { get; set; }
        
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Follower>().HasKey(favorite => new { favorite.TargetId, favorite.ObserverId });

            modelBuilder.Entity<Article>()
                .HasMany(article => article.Comments)
                .WithOne(comment => comment.Article)
                .HasForeignKey(comment => comment.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasOne(article => article.Author)
                .WithMany(author => author.Articles)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArticleFavorite>()
                .HasOne(favorite => favorite.Article)
                .WithMany(article => article.Favorites)
                .HasForeignKey(favorite => favorite.ArticleId);

            modelBuilder.Entity<ArticleFavorite>()
                .HasOne(favorite => favorite.Observer)
                .WithMany(observer => observer.Favorites)
                .HasForeignKey(favorite => favorite.ObserverId);
        }
    }
}