using Microsoft.AspNetCore.Identity;
using WebApplication1.Core.Models.Identity;

namespace WebApplication1.Core.Database.AppDb;

public class AppUserToken:IdentityUserToken<Guid>
{
    public AppUserToken(AppUserTokenModel tokenDto)
    {
        throw new NotImplementedException();
    }
}