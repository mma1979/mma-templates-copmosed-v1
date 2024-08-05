using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WebApplication1.Common.Extensions;
using WebApplication1.Core.Consts;
using WebApplication1.Services.AccountService.Models;

namespace WebApplication1.Infrastructure;

 public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app, TokenValidationParameters tokenValidationParameters)
        {
            app.Use(async (ctx, next) =>
            {
                if (ctx.GetEndpoint().Metadata.Any(e => e is AllowAnonymousAttribute))
                {
                    await next();
                }

                var token = ctx.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
                {
                    ctx.Response.StatusCode = 401;
                    await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("No Token Provided"));
                    await ctx.Response.Body.FlushAsync();
                    await next();
                }

                (bool isValid, string jwtToken) = DecryptAndValidate(token);
                if (!isValid)
                {
                    ctx.Response.StatusCode = 401;
                    await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Invalid Token"));
                    await ctx.Response.Body.FlushAsync();
                    await next();
                }

                //TODO: invock token from user tokens

                var splits = jwtToken.Split(' ');
                if (splits.Length < 2)
                {
                    ctx.Response.StatusCode = 401;
                    await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Invalid Token Type"));
                    await ctx.Response.Body.FlushAsync();
                    await next();
                }
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken tokenInfo = null;
                try
                {
                    handler.ValidateToken(splits[1], tokenValidationParameters, out SecurityToken validatedToken);

                    if (validatedToken == null)
                    {
                        throw new Exception();
                    }

                    tokenInfo = handler.ReadJwtToken(splits[1]);
                }
                catch (Exception)
                {

                    ctx.Response.StatusCode = 401;
                    await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Invalid Token"));
                    await ctx.Response.Body.FlushAsync();
                    await next();
                }

                // Expiration Handle
                long.TryParse(tokenInfo.Claims.FirstOrDefault(c => c.Type == "exp")?.Value, out var exp);

                var now = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                if (exp <= now)
                {
                    ctx.Response.StatusCode = 401;
                    await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Expired Token"));
                    await ctx.Response.Body.FlushAsync();
                    await next();
                }

                var roles = tokenInfo.Claims
                .Where(c => c.Type == "role")
                .Select(e => e.Value)
                .ToList();

                if (!roles.Any())
                {
                    ctx.Response.StatusCode = 401;
                    await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("No Roles"));
                    await ctx.Response.Body.FlushAsync();
                    await next();
                }

                var identity = new ClaimsPrincipal(new ClaimsIdentity(tokenInfo.Claims));
                ctx.User = identity;
                await next();

            });
            return app;
        }

        private static (bool isValid, string jwtToken) DecryptAndValidate(string token)
        {
            try
            {

                var secToken = token.Split(' ')[1];
                var secParts = secToken.Split('.');

                var connectionStr = GetValue<string>("ConnectionStrings:DefaultConnection");
                using var connection = new SqlConnection(connectionStr);
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
                var query = "select * from UserTokens where Name=@Name and UserId=@UserId and Value=@Value";
                var data = connection.Query<UserTokenDto>(query, new {Name= TokenTypes.LOGIN_TOKEN, UserId = secParts[^1], Value = secToken }).AsList();
                if (!data.Any()) {
                    return (false, string.Empty);
                }
                
                var decrypted = secParts[0].Decrypt();
                var hash = decrypted.ToUniqueNumericValue().ToString();
                return (hash == secParts[1], decrypted);
            }
            catch (Exception ex)
            {
                Log.Error("Invalid Token", ex, token);
                return (false, string.Empty);
            }
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app, string allowedHosts)
        {
            var origins = allowedHosts.Split(',')
                .Select(s => s.Trim());

            app.UseCors(opt =>
                opt.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(origin =>
                    {
                        if (allowedHosts == "*") return true;
                        //if (env.IsDevelopment()) return true;
                        // Only add this to allow testing with localhost, remove this line in production!  
                        if (origin.ToLower().Contains("localhost")) return true;
                        if (string.IsNullOrWhiteSpace(origin)) return false;


                        // Insert your production domain here.  
                        var allowedOrigin = origins.Any(x => origin.ToLower().StartsWith(x.ToLower()));
                        return allowedOrigin;
                    }));
            return app;
        }

        private static IConfiguration GetConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables();

            var configuration = builder.Build();


            return configuration;
        }

        private static T GetValue<T>(string key) {
            var configuration = GetConfiguration();
            return configuration.GetValue<T>(key);
        }
    }
