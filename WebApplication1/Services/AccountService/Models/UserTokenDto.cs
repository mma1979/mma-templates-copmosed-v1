using WebApplication1.Core.Models.Identity;

namespace WebApplication1.Services.AccountService.Models;

public class UserTokenDto
{
    public long Id { get; set; }
    public string Token { get; set; }
    public AppUserModel AppUser { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool? IsDeleted { get; set; }
    public long? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
    public long UserId { get; set; }
    public string LoginProvider { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}