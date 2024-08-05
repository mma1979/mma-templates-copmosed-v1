namespace WebApplication1.Services.AccountService.Models;

public abstract class RegisterDto(
    string username,
    string email,
    string countryCode,
    string password,
    string confirmPassword,
    string mobileNumber,
    string firstName,
    string lastName,
    IEnumerable<string> roles)
{
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
    public string CountryCode { get; set; } = countryCode;
    public string Password { get; set; } = password;
    public string ConfirmPassword { get; set; } = confirmPassword;
    public string MobileNumber { get; set; } = mobileNumber;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public IEnumerable<string> Roles { get; set; } = roles;
}