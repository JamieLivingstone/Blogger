using System;
using Blogger.WebApi.Resources.Profile;

namespace Blogger.WebApi.Resources.Article
{
    public class ArticleResource
    {
        public string Slug { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public string Body { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public ProfileResource Author { get; set; }
    }
}