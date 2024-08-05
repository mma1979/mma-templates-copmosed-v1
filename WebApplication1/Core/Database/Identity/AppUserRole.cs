﻿using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Core.Database.Identity;

public class AppUserRole: IdentityUserRole<Guid>
{
    public virtual AppUser AppUser { get; set; }
    public virtual AppRole AppRole { get; set; }
    
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool? IsDeleted { get; set; }
    public long? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(UserId,RoleId);
    }

    public override bool Equals(object obj)
    {
        return obj is AppUserRole other &&
               UserId == other.UserId &&
               RoleId == other.RoleId;
    }
}