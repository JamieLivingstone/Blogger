using System;
using Blogger.WebApi.Resources.Profile;

namespace Blogger.WebApi.Resources.Comment
{
    public class CommentResource
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Body { get; set; }

        public ProfileResource Author { get; set; }
    }
}