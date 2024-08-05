using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApplication1.Core.Database.Identity;
using WebApplication1.DataAccess.EntityConfigurations.Identity;

namespace WebApplication1.DataAccess;

public class AuthenticationDbContext : IdentityDbContext<AppUser, AppRole, Guid, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
{
    public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("name=AuthenticationConnection");
        }
    }
    
    public virtual DbSet<AppRefreshToken> AppRefreshTokens { get; set; }
    public virtual DbSet<AppRole> AppRoles { get; set; }
    public virtual DbSet<AppRoleClaim> AppRoleClaims { get; set; }
    public virtual DbSet<AppUser> AppUsers { get; set; }
    public virtual DbSet<AppUserClaim> AppUserClaims { get; set; }
    public virtual DbSet<AppUserLogin> AppUserLogins { get; set; }
    public virtual DbSet<AppUserRole> AppUserRoles { get; set; }
    public virtual DbSet<AppUserToken> AppUserTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.HasDefaultSchema("dbo");


        builder.ApplyConfiguration(new AppRefreshTokenConfig());
        builder.ApplyConfiguration(new AppRolesConfig());
        builder.ApplyConfiguration(new AppRoleClaimConfig());
        builder.ApplyConfiguration(new AppUserConfig());
        builder.ApplyConfiguration(new AppUserClaimConfig());
        builder.ApplyConfiguration(new AppUserLoginConfig());
        builder.ApplyConfiguration(new AppUserRoleConfig());
        builder.ApplyConfiguration(new AppUserTokenConfig());
        
        
        
        
    }
}