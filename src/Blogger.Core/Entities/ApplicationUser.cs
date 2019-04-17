using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Blogger.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Bio { get; set; }
        
        public string Image { get; set; }
        
        [NotMapped]
        public string Token { get; set; }
        
        [NotMapped]
        public bool Following { get; set; }
    }
}