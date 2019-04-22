using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blogger.Core.Entities
{
    public class Article : BaseEntity
    {
        public string Slug { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string AuthorId { get; set; }

        public ApplicationUser Author { get; set; }

        public List<Comment> Comments { get; set; }

        public List<ArticleFavorite> Favorites { get; set; }

        public List<ArticleTag> Tags { get; set; }

        [NotMapped]
        public bool Favorited { get; set; }

        [NotMapped]
        public int FavoritesCount { get; set; }
    }
}