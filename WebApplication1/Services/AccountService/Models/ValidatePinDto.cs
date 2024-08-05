namespace WebApplication1.Services.AccountService.Models;

public class ValidatePinDto(string email, string pin, string provider)
{
    public string Email { get; set; }
    public string Provider { get; set; }
    public string Pin { get; set; }
}