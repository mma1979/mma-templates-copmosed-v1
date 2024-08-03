using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApplication1.Core.Database.AppDb;

namespace WebApplication1.DataAccess;

public class ApplicationDbContext: IdentityDbContext<AppUser, AppRole, Guid, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
{
    
}