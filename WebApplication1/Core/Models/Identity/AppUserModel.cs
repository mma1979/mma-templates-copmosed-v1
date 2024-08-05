namespace WebApplication1.Core.Models.Identity;

public class AppUserModel
{
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public string NormalizedEmail { get; set; }
    public string NormalizedUserName { get; set; }
    public string SecurityStamp { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
}