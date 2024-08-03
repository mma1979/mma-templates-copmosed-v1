using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using WebApplication1.Common.Extensions;
using WebApplication1.Common.Models;

namespace WebApplication1.Common.Helpers
{
    public class PermissionsHelper
    {
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly RestHelper _restHelper;
        public PermissionsHelper(IOptions<AppSettings> options, IHttpContextAccessor contextAccessor, RestHelper restHelper)
        {
            _appSettings = options.Value;
            _contextAccessor = contextAccessor;
            _restHelper = restHelper;
        }


        public async Task<T> PagePermissions<T>(string pageCode)
        {
            var token = _contextAccessor.HttpContext.Request.Cookies["Token"];
            var handler = new JwtSecurityTokenHandler();
            var tokenInfo = handler.ReadJwtToken(token);
            var userId = tokenInfo.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value.ToNullableInt();

            var res = await _restHelper.Get<T>(_appSettings.APIEndPoint,
               $"api/Permissions/{userId}?pageCode={pageCode}").ConfigureAwait(false);

            return res;
        }
    }


}
