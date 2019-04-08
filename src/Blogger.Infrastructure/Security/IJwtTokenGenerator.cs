using Blogger.Core.Entities;

namespace Blogger.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        string CreateToken(ApplicationUser user);
    }
}