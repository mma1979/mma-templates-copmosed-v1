using Microsoft.AspNetCore.Identity;
using WebApplication1.Core.Models;

namespace WebApplication1.Core.Database.Identity;

public class AppRole: IdentityRole<Guid>
{
    
    public long? CreatedBy { get;  set; }
    public DateTime? CreatedDate { get;  set; }
    public long? ModifiedBy { get;  set; }
    public DateTime? ModifiedDate { get;  set; }
    public bool? IsDeleted { get;  set; }
    public long? DeletedBy { get;  set; }
    public DateTime? DeletedDate { get;  set; }
    
    public virtual ICollection<AppUserRole> AppUserRoles { get; set; }

    #region Actions

    private AppRole()
    {
        AppUserRoles = new HashSet<AppUserRole>();
    }
    public AppRole(AppRoleModel dto)
    {

        Name = dto.Name;
        CreatedDate = DateTime.UtcNow;

    }

    public AppRole Update(AppRoleModel dto)
    {
        Name = dto.Name;
        ModifiedDate = DateTime.UtcNow;
        return this;
    }

    public AppRole Delete()
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
        return this;
    }

    public AppRole(string? rolename)
    {
        Name = rolename;
    }

    

    #endregion
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }

    public override bool Equals(object obj)
    {
        return obj is AppRole other &&
               Name == other.Name;
    }
}