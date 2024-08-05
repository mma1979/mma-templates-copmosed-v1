using Microsoft.AspNetCore.Identity;
using WebApplication1.Core.Models.Identity;

namespace WebApplication1.Core.Database.Identity;

public class AppUserToken:IdentityUserToken<Guid>
{
    
    public string Token { get; private set; }
    public virtual AppUser AppUser { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? CreatedDate { get; private set; }
    public Guid? ModifiedBy { get; private set; }
    public DateTime? ModifiedDate { get; private set; }
    public bool? IsDeleted { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public DateTime? DeletedDate { get; private set; }
    
    public AppUserToken()
    {

    }
    public AppUserToken(AppUserTokenModel tokenDto)
    {
        Name = tokenDto.Name;
        LoginProvider = tokenDto.LoginProvider;
        Value = tokenDto.Value;
        Token = tokenDto.Token;

    }
}