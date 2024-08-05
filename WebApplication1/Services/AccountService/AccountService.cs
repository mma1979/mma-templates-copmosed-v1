using WebApplication1.Common.Extensions;
using WebApplication1.Core.Consts;
using WebApplication1.Core.Models;
using WebApplication1.DataAccess;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApplication1.Core.Database.Identity;
using WebApplication1.Core.Models.Identity;
using WebApplication1.Models;
using WebApplication1.ProxyServices.EmailService;
using WebApplication1.Services.AccountService.Models;

namespace WebApplication1.Services.AccountService;

public class AccountService(
    IServiceScopeFactory scopeFactory,
    ILogger<AccountService> logger,
    IOptionsMonitor<JwtConfig> optionsMonitor,
    TokenValidationParameters tokenValidationParameters)
    : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private ILogger<AccountService> _logger = logger;
    private readonly JwtConfig _jwtConfig = optionsMonitor.CurrentValue;
    private readonly TokenValidationParameters _tokenValidationParameters = tokenValidationParameters;
    
     public async Task<ResultViewModel<RegistrationResponse>> Register(RegisterDto model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return ResultViewModel<RegistrationResponse>.Error(messages: ["Passwords do not match!"]);
                
            }

            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var userExists = await userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
            if (userExists != null)
            {
                return ResultViewModel<RegistrationResponse>.Error(messages: ["User already exists!"]);
            }


            await using var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();

            AppUser user = new(new AppUserModel
            {
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                UserName = model.Username,
                NormalizedUserName = model.Username.ToUpper(),
                PhoneNumber = model.MobileNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            });



            var result = await userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return ResultViewModel<RegistrationResponse>.Error(messages:  result.Errors.Select(e => e.Description).ToList());
            }

            using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            foreach (string role in model.Roles)
            {
                if (!await roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                    await roleManager.CreateAsync(new AppRole(role)).ConfigureAwait(false);

                await userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
            }

            var jwtToken = await GenerateJwtToken(user);

            // send confirmation email 
            string confirmationToken = await userManager.
                 GenerateEmailConfirmationTokenAsync(user);


            string pin = confirmationToken.ExtractPin();
            user.AddUserToken(new AppUserTokenModel
            {
                Value = pin,
                Token = confirmationToken,
                LoginProvider = TokenTypes.EMAIL_CONFIRMATION,
                Name = TokenTypes.EMAIL_CONFIRMATION
            });

            _ = await context.SaveChangesAsync();

            //send email
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
            _ = emailService.Send(user.Email!, TokenTypes.EMAIL_CONFIRMATION, $"Your pin code is: {pin}");

            return ResultViewModel<RegistrationResponse>.Success(new()
            {
                RefreshToken = jwtToken.RefreshToken,
                Token = jwtToken.Token
            }, messages: new() { "User created successfully" });
            
        }
     
        public async Task<ResultViewModel<TokenDto>> Login(LoginDto model)
        {
            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<AppUser>>();

            var user = await userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
            if (user == null)
            {
                return ResultViewModel<TokenDto>.Error(messages: new() { "User does not exist!" });
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return ResultViewModel<TokenDto>.Error(messages: new() { "Invalid password!" });
            }

            var jwtToken = await GenerateJwtToken(user);
            return ResultViewModel<TokenDto>.Success(new() { RefreshToken = jwtToken.RefreshToken, Token = jwtToken.Token }, messages: new()
                { "User logged in successfully" });
        }
        
        public async Task<AcknowledgeViewModel> Logout(Guid userId)
        {
            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<AppUser>>();

            var user = await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                return AcknowledgeViewModel.Error(messages: ["User does not exist!"]);
            }

            await using var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
            var userTokens = context.AppUserTokens.Where(t=>t.UserId== userId && t.Name== TokenTypes.LOGIN_TOKEN).ToList();
            context.UserTokens.RemoveRange(userTokens);
            _ = await context.SaveChangesAsync();

            return AcknowledgeViewModel.Success(messages: ["User logged out"]);
            
        }
        
        public async Task<ResultViewModel<TokenDto>> ResetPassword(ResetPasswordDto model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                return ResultViewModel<TokenDto>.Error(
                    new() { "Passwords do not match!" });
            }

            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
            if (user == null)
            {
                return ResultViewModel<TokenDto>.Error(messages: ["User does not exist!"]);
            }



            var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return ResultViewModel<TokenDto>.Error(messages: result.Errors.Select(e => e.Description).ToList());
            }

            var jwtToken = await GenerateJwtToken(user);
            return ResultViewModel<TokenDto>.Success(new TokenDto { RefreshToken = jwtToken.RefreshToken, Token = jwtToken.Token },
                messages: new() { "Password reset successfully" });
        }
        
        public async Task<AcknowledgeViewModel> EnableTwoFactorAuthentication(EnableTwoFactorAuthenticationDto model)
        {
            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByNameAsync(model.UserName).ConfigureAwait(false);
            if (user == null)
            {
                return AcknowledgeViewModel.Error(messages: new() { "User does not exist!" });
            }

            var result = await userManager.SetTwoFactorEnabledAsync(user, model.Enabled).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return AcknowledgeViewModel.Error(messages: result.Errors.Select(e => e.Description).ToList());
            }

            return AcknowledgeViewModel.Success(messages: ["Two factor authentication enabled successfully"]);
        }
        
        public async Task<ResultViewModel<string>> GenerateOtp(string userName)
        {
            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null)
            {
                return new ResultViewModel<string>
                {
                    IsSuccess = false,
                    Messages = { "User does not exist!" }
                };
            }

            var otp = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber).ConfigureAwait(false);
            return new ResultViewModel<string>
            {
                IsSuccess = true,
                Data = otp
            };
        }
        
        public async Task<AcknowledgeViewModel> ValidateOtp(string userName, long otp)
        {
            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null)
            {
                return new AcknowledgeViewModel
                {
                    IsSuccess = false,
                    Messages = { "User does not exist!" }
                };
            }

            var result = await userManager.VerifyTwoFactorTokenAsync(user, "Phone", otp.ToString()).ConfigureAwait(false);
            if (!result)
            {
                return new AcknowledgeViewModel
                {
                    IsSuccess = false,
                    Messages = { "Invalid OTP!" }
                };
            }

            return new AcknowledgeViewModel
            {
                IsSuccess = true,
                Messages = { "OTP validated successfully" }
            };
        }
        
        public async Task<ResultViewModel<TokenDto>> VerifyToken(TokenDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                // Now we need to check if the token has a valid security algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return new()
                        {
                            IsSuccess = false,
                            Messages = { "Invalid Token" }
                        };
                    }
                }

                // Now we need to check if the token has a valid date
                var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expDate > DateTime.UtcNow)
                {
                    return new()
                    {
                        Messages = { "We cannot refresh this since the token has not expired" },
                        IsSuccess = false
                    };
                }

                // Check the token we got if it's saved in the db
                using var scope = _scopeFactory.CreateScope();
                await using var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
                var storedRefreshToken = await context.AppRefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new()
                    {
                        Messages = { "refresh token doesnt exist" },
                        IsSuccess = false
                    };
                }

                // Check the date of the saved token if it has expired
                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new()
                    {
                        Messages = { "token has expired, user needs to relogin" },
                        IsSuccess = false
                    };
                }

                // check if the refresh token has been used
                if (storedRefreshToken.IsUsed)
                {
                    return new()
                    {
                        Messages = { "token has been used" },
                        IsSuccess = false
                    };
                }

                // Check if the token is revoked
                if (storedRefreshToken.IsRevoked)
                {
                    return new()
                    {
                        Messages = { "token has been revoked" },
                        IsSuccess = false
                    };
                }

                // we are getting here the jwt token id
                var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;

                // check the id that the recieved token has against the id saved in the db
                if (storedRefreshToken.JwtId != jti)
                {
                    return new()
                    {
                        Messages = { "the token doenst mateched the saved token" },
                        IsSuccess = false
                    };
                }

                storedRefreshToken.MarkAsUsed();
                context.AppRefreshTokens.Update(storedRefreshToken);
                await context.SaveChangesAsync();
                using var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId.ToString());
                var genratedToken = await GenerateJwtToken(dbUser);

                return new()
                {
                    IsSuccess = true,
                    Data = new()
                    {
                        Token = genratedToken.Token,
                        RefreshToken = genratedToken.RefreshToken,
                    }
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.UtcNow} - {nameof(VerifyToken)}", tokenRequest, ex);
                return new()
                {
                    Messages = { "can not validate the token" },
                    IsSuccess = false
                };
            }
        }
        
        
         public async Task<AcknowledgeViewModel> ConfirmEmail(long userId, string pin)
        {
            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            await using var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();

            var user = await userManager.FindByIdAsync(userId.ToString());
            var userToken = await context.UserTokens.FirstOrDefaultAsync(e => e.LoginProvider == TokenTypes.EMAIL_CONFIRMATION && e.Value == pin);
            if (userToken == null)
            {
                _logger.LogInformation($"{DateTime.UtcNow} - Info on {nameof(ConfirmEmail)}: Token does not exist", userId, pin);
                return new()
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Messages = { "Token does not exist" }
                };
            }

            user.AppUserTokens.Remove(userToken);
            IdentityResult result = await userManager.
                ConfirmEmailAsync(user, userToken.Token);

            if (!result.Succeeded)
            {
                _logger.LogInformation($"{DateTime.UtcNow} - Info on {nameof(ConfirmEmail)}: Error while confirming your email", userId, pin, userToken);
                return new()
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Messages = { "Error while confirming your email" }
                };
            }

            _ = await context.SaveChangesAsync();

            return new()
            {
                IsSuccess = true,
                StatusCode = 200,
                Messages = { "Email successfully confirmed" }
            };
        }

        public async Task<AcknowledgeViewModel> ForgetPassword(ForgotPasswordDto model)
        {

            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByNameAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                return new()
                {
                    IsSuccess = false,
                    Messages = { "User does not exist!" },
                    StatusCode = 500
                };
            }
            using var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();

            var token = await userManager
                .GeneratePasswordResetTokenAsync(user);

            string pin = token.ExtractPin();
            user.AddUserToken(new AppUserTokenModel
            {
                Value = pin,
                Token = token,
                LoginProvider = TokenTypes.FORGET_PASSWORD_TOKEN,
                Name = TokenTypes.FORGET_PASSWORD_TOKEN
            });

            _ = await context.SaveChangesAsync();

            //send email
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
            _ = emailService.Send(user.Email, TokenTypes.FORGET_PASSWORD_TOKEN, $"Your pin code is: {pin}");


            return new()
            {
                IsSuccess = true,
                Messages = { "Check your email to reset your password" },
                StatusCode = 200
            };

        }

        public async Task<AcknowledgeViewModel> PinValidate(ValidatePinDto model)
        {

            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            await using var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();

            var user = await userManager.FindByEmailAsync(model.Email);
            var userToken = await context.UserTokens.FirstOrDefaultAsync(e => e.LoginProvider == model.Provider && e.Value == model.Pin);
            if (userToken == null)
            {
                _logger.LogInformation($"{DateTime.UtcNow} - Info on {nameof(ConfirmEmail)}: Token does not exist", model);
                return new()
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Messages = { "Token does not exist" }
                };
            }

            user.AppUserTokens.Remove(userToken);
            IdentityResult result = await userManager.
                ConfirmEmailAsync(user, userToken.Token);

            if (!result.Succeeded)
            {
                _logger.LogInformation($"{DateTime.UtcNow} - Info on {nameof(ConfirmEmail)}: Error while confirming your email", model);
                return new()
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Messages = { "Error while confirming your email" }
                };
            }

            _ = await context.SaveChangesAsync();

            return new()
            {
                IsSuccess = true,
                StatusCode = 200,
                Messages = { "Email successfuly confirmed" }
            };

        }
        
     
      private async Task<TokenDto> GenerateJwtToken(AppUser user)
        {
            // Now its ime to define the jwt token which will be responsible of creating our tokens
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // We get our secret from the appsettings
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            using var scope = _scopeFactory.CreateScope();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            var userRoles = await userManager.GetRolesAsync(user).ConfigureAwait(false);

            // we define our token descriptor
            // We need to utilise claims which are properties in our token which gives information about the token
            // which belong to the specific user who it belongs to
            // so it could contain their id, name, email the good part is that these information
            // are generated by our server and identity framework which is valid and trusted

            var authClaims = new List<Claim>
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                     // the JTI is used for our refresh token 
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                // the life span of the token needs to be shorter and utilise refresh token to keep the user signedin
                // but since this is a demo app we can extend it to fit our current need
                Expires = DateTime.UtcNow.AddHours(_jwtConfig.Expire),
                // here we are adding the encryption alogorithim information which will be used to decrypt our token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new AppRefreshToken(new()
            {

                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsRevoked = false,
                Token = RandomString(25) + Guid.NewGuid()

            });

            using var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
            await context.AppRefreshTokens.AddAsync(refreshToken);
            

            var secToken = $"{jwtToken.Encrypt()}.{jwtToken.ToUniqueNumericValue()}.{user.Id}";

            await context.UserTokens.AddAsync(new(new()
            {
                UserId = user.Id,
                LoginProvider = TokenTypes.LOGIN_TOKEN,
                Token = jwtToken,
                Value = secToken,
                Name = TokenTypes.LOGIN_TOKEN
            }));

            _ = await context.SaveChangesAsync();

            return new()
            {
                Token = secToken,
                RefreshToken = refreshToken.Token
            };
        }
      
      private string RandomString(int length)
      {
          var random = new Random();
          var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
          return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
      }
      
      private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
      {
          // Unix timestamp is seconds past epoch
          System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
          dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
          return dtDateTime;
      }


    #region IDisposable
    public void Dispose(bool disposing)
    {
        if (!disposing) return;
        Dispose();
        GC.Collect();
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


    #endregion
}