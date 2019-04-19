using System;

namespace Blogger.Core.Entities
{
    public class Comment : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string Body { get; set; }
        
        public int AuthorId { get; set; }
        
        public ApplicationUser Author { get; set; }
        
        public int ArticleId { get; set; }
        
        public Article Article { get; set; }
    }
}