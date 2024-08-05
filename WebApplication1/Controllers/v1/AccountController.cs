using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Extensions;
using WebApplication1.Common.Helpers;
using WebApplication1.Core.Consts;
using WebApplication1.Services.AccountService;
using WebApplication1.Services.AccountService.Models;

namespace WebApplication1.Controllers.v1;

public class AccountController(
    IServiceScopeFactory scopeFactory,
    ILogger<AccountController> logger,
    Translator translator)
    : BaseController(translator)
{
    private readonly ILogger<AccountController> _logger = logger;
    private readonly Translator _translator = translator;
    private static readonly string[] error = new[] { "Request has been canceled" };

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = new[] { "Request has been canceled" }
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = ModelState.Values //TODO: GetErrors
            });
        }

        using var scope = scopeFactory.CreateScope();
        using var service = scope.ServiceProvider.GetRequiredService<AccountService>();
        var result = await service.Register(registerDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }


    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmCmail([FromQuery] long userId, [FromQuery] string pin,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = new[] { "Request has been canceled" }
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = ModelState.Values //TODO: GetErrors
            });
        }

        using var scope = scopeFactory.CreateScope();
        using var service = scope.ServiceProvider.GetRequiredService<AccountService>();
        var result = await service.ConfirmEmail(userId, pin);
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = new[] { "Request has been canceled" }
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = ModelState.Values //TODO: GetErrors
            });
        }

        using var scope = scopeFactory.CreateScope();
        using var service = scope.ServiceProvider.GetRequiredService<AccountService>();
        var result = await service.Login(loginDto);
        if (result.IsSuccess)
        {
            HttpContext.Request.Cookies.Keys.ToList().ForEach(x => { Response.Cookies.Delete(x); });

            CookieOptions option = new CookieOptions()
            {
                Path = "/",
                HttpOnly = true,
                IsEssential = true,
                Secure = true
            };

            Response.Cookies.Append("Token", result.Data.Token, option);
            Response.Cookies.Append("Refresh-Token", result.Data.RefreshToken, option);

            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = new[] { "Request has been canceled" }
            });
        }


        using var scope = scopeFactory.CreateScope();
        using var service = scope.ServiceProvider.GetRequiredService<AccountService>();
       Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId);
        var result = await service.Logout(userId);
        if (result.IsSuccess)
        {
            HttpContext.Request.Cookies.Keys.ToList().ForEach(x => { Response.Cookies.Delete(x); });

            return Ok(result);
        }

        return BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordDto forgetPasswordDto,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = new[] { "Request has been canceled" }
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = ModelState.Values //TODO: GetErrors
            });
        }


        using var scope = scopeFactory.CreateScope();
        using var service = scope.ServiceProvider.GetRequiredService<AccountService>();
        var result = await service.ForgetPassword(forgetPasswordDto);
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("forget-password-validate")]
    public async Task<IActionResult> ForgetPasswordValidate([FromBody] ForgotPasswordDto dto,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = error
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = ModelState.Values //TODO: GetErrors
            });
        }


        using var scope = scopeFactory.CreateScope();
        using var service = scope.ServiceProvider.GetRequiredService<AccountService>();
        var result = await service.PinValidate(new(dto.Email, dto.Pin, TokenTypes.FORGET_PASSWORD_TOKEN));
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = new[] { "Request has been canceled" }
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = ModelState.Values //TODO: GetErrors
            });
        }

        using var scope = scopeFactory.CreateScope();
        using var service = scope.ServiceProvider.GetRequiredService<AccountService>();
        var result = await service.ResetPassword(resetPasswordDto);
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);
    }


    [HttpPost("validate-otp")]
    public async Task<IActionResult> ValidateOTP([FromBody] ValidateOtpDto validateOtpDto,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = new[] { "Request has been canceled" }
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = ModelState.Values //TODO: GetErrors
            });
        }

        using var scope = scopeFactory.CreateScope();
        using var service = scope.ServiceProvider.GetRequiredService<AccountService>();
        var result = await service.ValidateOtp(validateOtpDto.Username, validateOtpDto.Otp);
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenRequest, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = new[] { "Request has been canceled" }
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Messages = ModelState.Values //TODO: GetErrors
            });
        }

        using var scope = scopeFactory.CreateScope();
        using var service = scope.ServiceProvider.GetRequiredService<AccountService>();

        var result = await service.VerifyToken(tokenRequest);
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);
    }
}