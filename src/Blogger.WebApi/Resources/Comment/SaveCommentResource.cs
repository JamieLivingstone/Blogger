using System.ComponentModel.DataAnnotations;

namespace Blogger.WebApi.Resources.Comment
{
    public class SaveCommentResource
    {
        [Required]
        [MinLength(3)]
        public string Body { get; set; }
    }
}