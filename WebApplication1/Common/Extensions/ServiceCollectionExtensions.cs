namespace WebApplication1.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        //public static IServiceCollection AddRestHelper(this IServiceCollection serviceCollection, Action<RestHelper> optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Singleton, ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        //{
        //    var httpContext = serviceCollection.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>().HttpContext;
        //    var token = httpContext?.Request.Cookies["Token"] ?? "";
        //    var restHelper = new RestHelper
        //    {
        //        Token = string.IsNullOrEmpty(token) ? token : $"Bearer {token}"
        //    };
        //    if (optionsAction != null) optionsAction(restHelper);
        //    serviceCollection.TryAdd(new ServiceDescriptor(typeof(RestHelper), typeof(RestHelper), optionsLifetime));
        //    return serviceCollection;
        //}
    }
}
