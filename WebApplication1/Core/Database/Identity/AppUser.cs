using Microsoft.AspNetCore.Identity;
using WebApplication1.Core.Enums;
using WebApplication1.Core.Models.Identity;

namespace WebApplication1.Core.Database.Identity;

public class AppUser:IdentityUser<Guid>
{
    public TwoFactorMethods TwoFactorMethod { get; set; }
    
    public long? CreatedBy { get;  set; }
    public DateTime? CreatedDate { get;  set; }
    public long? ModifiedBy { get;  set; }
    public DateTime? ModifiedDate { get;  set; }
    public bool? IsDeleted { get;  set; }
    public long? DeletedBy { get;  set; }
    public DateTime? DeletedDate { get;  set; }
    
    
    public virtual ICollection<AppUserRole> AppUserRoles { get; private set; }
    public virtual ICollection<AppUserToken> AppUserTokens { get; private set; }
    public virtual ICollection<AppRefreshToken> AppRefreshTokens { get; private set; }
    
    
    

        #region Actions
        private AppUser()
        {
            AppUserRoles = new HashSet<AppUserRole>();
            AppUserTokens = new HashSet<AppUserToken>();
            AppRefreshTokens = new HashSet<AppRefreshToken>();

        }

        public AppUser(AppUserModel dto)
        {
            Email = dto.Email;
            NormalizedEmail = dto.Email.ToUpper();
            UserName = dto.UserName;
            NormalizedUserName = dto.UserName.ToUpper();
            PhoneNumber = dto.PhoneNumber;
            SecurityStamp = Guid.NewGuid().ToString();
            TwoFactorEnabled = false;
            TwoFactorMethod = Enums.TwoFactorMethods.None;

            CreatedDate = DateTime.UtcNow;
            CreatedBy = 1;

        }

        public AppUser Update(AppUserModel dto)
        {
            Email = dto.Email;
            NormalizedEmail = dto.Email.ToUpper();
            UserName = dto.UserName;
            NormalizedUserName = dto.UserName.ToUpper();
            PhoneNumber = dto.PhoneNumber;
            SecurityStamp = Guid.NewGuid().ToString();
            
            CreatedDate = DateTime.UtcNow;
            CreatedBy = 1;

            return this;
        }

        public AppUser SetRole(AppRoleModel role)
        {
            AppUserRoles = new HashSet<AppUserRole>
            {
                new AppUserRole{RoleId=role.Id}
            };
            CreatedDate = DateTime.UtcNow;
            CreatedBy = 1;

            return this;
        }

        public AppUser AddUserRoles(HashSet<AppUserRoleModel> userRoles)
        {
            AppUserRoles = userRoles.Select(r => new AppUserRole { RoleId = r.RoleId }).ToHashSet();
            CreatedDate = DateTime.UtcNow;
            CreatedBy = 1;

            return this;
        }


        public AppUser AddUserToken(AppUserTokenModel tokenDto)
        {
            AppUserTokens ??= new HashSet<AppUserToken>();
            AppUserTokens.Add(new AppUserToken(tokenDto));
            return this;
        }

        
        public AppUser Delete()
        {
            IsDeleted = true;
            DeletedDate = DateTime.UtcNow;
            
            return this;
        }
        #endregion

 
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, UserName, Email, PhoneNumber);
    }
    public override bool Equals(object obj)
    {
        return obj is AppUser other &&
               Id == other.Id &&
               UserName == other.UserName &&
               Email == other.Email &&
               PhoneNumber == other.PhoneNumber ;
    }
}