using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Core.Database.Identity;

public class AppUserClaim:IdentityUserClaim<Guid>
{
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool? IsDeleted { get; set; }
    public long? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
}