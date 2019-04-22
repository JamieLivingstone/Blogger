using System.ComponentModel.DataAnnotations;

namespace Blogger.WebApi.Resources.User
{
    public class LoginUserResource
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}