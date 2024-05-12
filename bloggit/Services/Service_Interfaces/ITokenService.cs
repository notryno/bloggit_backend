using bloggit.Models;

namespace bloggit.Services.Service_Implements
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user, IList<string> role);
    }
}
