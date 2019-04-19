using System;
using System.Collections.Generic;

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
        
        public ApplicationUser Author { get; set; }
        
        public List<Comment> Comments { get; set; }
    }
}