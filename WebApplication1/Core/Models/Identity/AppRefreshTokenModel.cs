namespace WebApplication1.Core.Models.Identity;

public class AppRefreshTokenModel
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string JwtId { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int Hash { get; set; }
    public AppUserModel AppUser { get; set; }
    public Guid Id { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool? IsDeleted { get; set; }
    public long? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
}