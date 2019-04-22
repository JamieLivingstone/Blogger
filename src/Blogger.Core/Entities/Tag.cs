using System.Collections.Generic;

namespace Blogger.Core.Entities
{
    public class Tag
    {
        public string Id { get; set; }

        public List<ArticleTag> ArticleTags { get; set; }

        public Tag(string id)
        {
            Id = id;
        }
    }
}