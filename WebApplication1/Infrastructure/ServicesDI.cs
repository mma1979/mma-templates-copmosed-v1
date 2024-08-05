using Microsoft.Extensions.Caching.Memory;
using WebApplication1.ProxyServices.EmailService;
using WebApplication1.Services.AccountService;

namespace WebApplication1.Infrastructure;

public static class ServicesDI
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
            
        //services.AddStripeService();
        services.AddTransient<IMemoryCache, MemoryCache>();
        // services.AddRedisCacheService();


        // services.AddTransient<LocalizationSerivce>();
        // services.AddTransient<SysSettingsService>();
        services.AddTransient<AccountService>();

        services.AddTransient<EmailService>();
        // services.AddTransient<RoleService>();
        // services.AddTransient<AttachmentsService>();

        return services;
    }
}