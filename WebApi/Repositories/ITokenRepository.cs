using Microsoft.AspNetCore.Identity;

namespace WebApi.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
