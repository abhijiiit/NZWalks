using Microsoft.AspNetCore.Identity;

namespace NZWalks.Repositories;

public interface ITokenRepository
{
    string CreateJwtToken(IdentityUser user, List<string> roles);  
}
        