namespace WebApplication1.Services.AccountService.Models;

public class JwtConfig
{
    public string ValidAudience { get; set; }
    public string ValidIssuer { get; set; }
    public string Secret { get; set; }
    public int Expire { get; set; }
}