using System.Globalization;
using Microsoft.Extensions.Localization;
using WebApplication1.Common.Extensions;
using WebApplication1.Common.Helpers;

namespace WebApplication1.Common.Componenets
{
    public class IOPTStringLocalaizer(Translator translator, IHttpContextAccessor httpContextAccessor)
        : IStringLocalizer
    {
        private readonly string _language = httpContextAccessor.HttpContext?.Request.Cookies["lang"] ?? "ar";

        public LocalizedString this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
              
                var value = translator.Translate(key, _language);
                return new LocalizedString(key, value);
            }
        }

        public LocalizedString this[string key, params object[] arguments]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                var args = (!string.IsNullOrEmpty(_language) ? _language :
                    arguments.HasAny() ? arguments[0].ToString() : "ar")!;

                var value = translator.Translate(key, args);
                return new LocalizedString(key, value);
            }
        }
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
