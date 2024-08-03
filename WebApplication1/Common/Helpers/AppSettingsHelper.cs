using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApplication1.Common.Models;

namespace WebApplication1.Common.Helpers
{
    public class AppSettingsHelper
    {
        public string SingleEndPoint { get; set; }
        public string DictEndPoint { get; set; }
        public string BaseUrl { get; set; }
        public bool? IsAsync { get; set; }

        public RestHelper RestHelper { get; set; }

        public AppSettingsHelper()
        {
            var config = GetConfiguration();
            BaseUrl ??= config.GetValue<string>("BaseUrl");
            DictEndPoint ??= config.GetValue<string>("DictEndPoint");
            SingleEndPoint ??= config.GetValue<string>("SingleEndPoint");
            IsAsync ??= config.GetValue<bool>("IsAsync");

        }
        public AppSettingsHelper(IConfigurationSection config)
        {
            BaseUrl ??= config.GetValue<string>("BaseUrl");
            DictEndPoint ??= config.GetValue<string>("DictEndPoint");
            SingleEndPoint ??= config.GetValue<string>("SingleEndPoint");
            IsAsync ??= config.GetValue<bool>("IsAsync");

        }

        private static IConfigurationSection GetConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);

            var Configuration = builder.Build();

            var config = Configuration.GetSection("SysSettings");

            return config;
        }


        public string GetAppSetting(string key)
        {
            var keySegmint = Tuple.Create<string, object>("key", key);
            var res = RestHelper.Get<ScalerResult>(BaseUrl,
                $"{SingleEndPoint}/{{key}}",
               urlSegment: new List<Tuple<string, object>> { keySegmint }).Result;
            return res.Value;
        }

        public async Task<string> GetAppSettingAsync(string key)
        {
            var keySegmint = Tuple.Create<string, object>("key", key);
            var res = await RestHelper.Get<ScalerResult>(BaseUrl,
                $"{SingleEndPoint}Async/{{key}}",
               urlSegment: new List<Tuple<string, object>> { keySegmint });
            return res.Value;
        }

        public Dictionary<string, string> GetAppSettings(params string[] keys)
        {
            var keysParams = string.Join('&', keys.Select(k => $"keys={k}"));
            var res = RestHelper.Get<Dictionary<string, string>>(BaseUrl,
               $"{DictEndPoint}?{keysParams}").Result;
            return res;
        }

        public async Task<Dictionary<string, string>> GetAppSettingsAsync(params string[] keys)
        {
            var keysParams = string.Join('&', keys.Select(k => $"keys={k}"));
            var res = await RestHelper.Get<Dictionary<string, string>>(BaseUrl,
               $"{DictEndPoint}Async?{keysParams}");
            return res;
        }


    }

    public static class AppSettingsHelperExtensions
    {
        public static IServiceCollection AddSysSettingsHelper(this IServiceCollection serviceCollection, Action<AppSettingsHelper> optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {

            var instance = new AppSettingsHelper();
            if(optionsAction != null)
                optionsAction(instance);
            serviceCollection.TryAdd(
                new ServiceDescriptor(typeof(AppSettingsHelper),
                provider => instance,
                optionsLifetime));

            return serviceCollection;
        }
    }
}
