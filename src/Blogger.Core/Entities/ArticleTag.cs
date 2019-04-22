namespace Blogger.Core.Entities
{
    public class ArticleTag : BaseEntity
    {
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        public string TagId { get; set; }
        public Tag Tag { get; set; }
    }
}