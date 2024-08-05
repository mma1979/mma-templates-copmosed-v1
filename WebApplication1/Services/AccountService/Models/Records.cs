namespace WebApplication1.Services.AccountService.Models;

public record LoginDto(string Username,string Password);
public record ForgotPasswordDto(string Email, string Pin);
public record ValidatePenDto(string Email, string Pen, string Provider);
public record ResetPasswordDto(string Username,string NewPassword,string ConfirmPassword,string Token);
public record ValidateOtpDto(string Username,string Password, long Otp);
public record ChangePasswordDto(string OldPassword, string NewPassword,string Confirmpassword);