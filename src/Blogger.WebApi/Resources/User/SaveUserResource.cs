using System.ComponentModel.DataAnnotations;

namespace Blogger.WebApi.Resources.User
{
    public class SaveUserResource
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}