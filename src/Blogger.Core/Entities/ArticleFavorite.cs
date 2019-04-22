namespace Blogger.Core.Entities
{
    public class ArticleFavorite : BaseEntity
    {
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        public string ObserverId { get; set; }
        public ApplicationUser Observer { get; set; }
    }
}