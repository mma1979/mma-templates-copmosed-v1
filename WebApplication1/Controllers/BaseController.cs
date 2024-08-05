using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Extensions;
using WebApplication1.Common.Helpers;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController(Translator translator) : ControllerBase
{
    private Translator _translator = translator;

    protected long? UserId
    {
        get
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
            {
                return null;
            }
            var splits = token.Split(' ');
            if (splits.Length < 2)
            {
                return null;
            }
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken tokenInfo;
            try
            {
                tokenInfo = handler.ReadJwtToken(splits[1]);
            }
            catch (Exception)
            {

                return null;
            }

            return tokenInfo.Claims.FirstOrDefault(c => c.Type == "Id")?.Value.ToNullableLong();

        }
    }

    protected string Language
    {
        get
        {
            var lang = string.IsNullOrWhiteSpace(HttpContext.Request.Headers["Accept-Language"]) ?
                "en" : HttpContext.Request.Headers["Accept-Language"].ToString();
            return lang;

        }
    }

}