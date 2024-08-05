namespace WebApplication1.Core.Models.Identity;

public class AppUserTokenModel
{
    public string Name { get; set; }
    public string LoginProvider { get; set; }
    public string? Value { get; set; }
    public string? Token { get; set; }
    public long? CreatedBy { get; set; }
    public Guid UserId { get; set; }
}