using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Core.Database.Identity;

public class AppUserClaim:IdentityUserClaim<Guid>
{
    public Guid? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool? IsDeleted { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
}