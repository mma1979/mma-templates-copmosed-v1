using System.Globalization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using WebApplication1.Common.Extensions;
using WebApplication1.Common.Helpers;

namespace WebApplication1.Common.Componenets
{
    public class IOPTViewLocalizer(Translator translator, IHttpContextAccessor httpContextAccessor)
        : IViewLocalizer
    {
        public LocalizedHtmlString this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                var lang = httpContextAccessor.HttpContext?.Request.Cookies[CookiesNames.Language] ?? "en";

                var value = translator.Translate(key, lang);
                return new LocalizedHtmlString(key, value);
            }
        }

        public LocalizedHtmlString this[string key, params object[] arguments]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                var lang = httpContextAccessor.HttpContext?.Request.Cookies[CookiesNames.Language];
                var args = (!string.IsNullOrEmpty(lang) ? lang :
                   arguments.HasAny() ? arguments[0].ToString() : "en")!;

                var value = translator.Translate(key, args);
                return new LocalizedHtmlString(key, value);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public LocalizedString GetString(string key)
        {
            var value = translator.Translate(key);
            return new LocalizedString(key, value);
        }

        public LocalizedString GetString(string key, params object[] arguments)
        {
            var value = translator.Translate(key);
            return new LocalizedString(key, value);
        }

        public IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
