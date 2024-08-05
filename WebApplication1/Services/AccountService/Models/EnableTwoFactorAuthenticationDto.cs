namespace WebApplication1.Services.AccountService.Models;

public class EnableTwoFactorAuthenticationDto
{
    public string UserName { get; set; }
    public bool Enabled { get; set; }
}