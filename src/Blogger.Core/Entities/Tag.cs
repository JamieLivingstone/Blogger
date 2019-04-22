using System.Collections.Generic;

namespace Blogger.Core.Entities
{
    public class Tag
    {
        public string TagId { get; set; }

        public List<ArticleTag> ArticleTags { get; set; }

        public Tag(string tagId)
        {
            TagId = tagId;
        }
    }
}