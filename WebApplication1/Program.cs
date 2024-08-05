using System.Net;
using System.Net.Mail;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Common.Models;
using WebApplication1.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Sinks.MSSqlServer;
using Swashbuckle.AspNetCore.SwaggerUI;
using WebApplication1.Common.Helpers;
using WebApplication1.Core.Database.Identity;
using WebApplication1.DataAccess;
using WebApplication1.Services.AccountService.Models;
using Redis = StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Host.UseSerilog((context, config) =>
{
    config.Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            connectionString: context.Configuration.GetConnectionString("AppLogsConnection"),
            sinkOptions: new MSSqlServerSinkOptions { TableName = "AppLogs_API", AutoCreateSqlTable = true }
        )

        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .Enrich.WithProperty("ApplicationName", context.Configuration["Serilog:ApplicationName"])
        .Enrich.WithEnvironmentUserName()
        .ReadFrom.Configuration(context.Configuration);
    //.CreateLogger();
});
    
builder.Services.AddAutoMapper(System.Reflection.Assembly.GetAssembly(typeof(MappingProfile)));

// configure strongly typed settings objects
IConfigurationSection appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);


builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();


#region API Versioning
builder.Services.AddApiVersioning(c =>
{
    c.DefaultApiVersion = new ApiVersion(1, 0);
    c.AssumeDefaultVersionWhenUnspecified = true;
    c.ReportApiVersions = true;
    c.ApiVersionReader = new UrlSegmentApiVersionReader();
});
#endregion

#region  For Entity Framework
builder.Services.AddDbContext<AuthenticationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AuthenticationConnection")));

#endregion

#region Authentication & JWT

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWT"));
// add validation paramter as singletone so that we can use it across the appications
TokenValidationParameters tokenValidationParameters = new()
{
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!)),
    ValidateIssuerSigningKey = true,
    ValidateLifetime = true,
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidAudience = builder.Configuration["JWT:ValidAudience"],
    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
    RequireExpirationTime = false,
    // Allow to use seconds for expiration of token
    // Required only when token lifetime less than 5 minutes
    // THIS ONE
    ClockSkew = TimeSpan.Zero

};

builder.Services.AddSingleton(tokenValidationParameters);

// Adding Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    // Adding Jwt Bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = tokenValidationParameters;
    });

// For Identity
builder.Services.AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AuthenticationDbContext>()
    .AddDefaultTokenProviders();



#endregion

#region Redis Cache

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{builder.Configuration.GetValue<string>("Redis:Server")}:{builder.Configuration.GetValue<int>("Redis:Port")}";
    options.ConfigurationOptions = Redis.ConfigurationOptions.Parse(options.Configuration);
    options.ConfigurationOptions.Password = builder.Configuration.GetValue<string>("Redis:Password");

});

#endregion

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mma.MySolution.AppApi", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}

        }
    });
});
#endregion

builder.Services
    .AddFluentEmail(builder.Configuration.GetValue<string>("SMTP:Email"))
    .AddRazorRenderer()
    .AddSmtpSender(new SmtpClient()
    {
        Host = builder.Configuration.GetValue<string>("SMTP:SmtpServer"),
        Port = builder.Configuration.GetValue<int>("SMTP:Port"),
        Credentials = new NetworkCredential(builder.Configuration.GetValue<string>("SMTP:Email"), builder.Configuration.GetValue<string>("SMTP:Password")),
        EnableSsl = builder.Configuration.GetValue<bool>("SMTP:EnableSsl")
    });

builder.Services.AddSignalR();
builder.Services.AddSysSettingsHelper();
builder.Services.AddTranslator();
builder.Services.AddServices();
builder.Services.AddTransient<RequestSanitizationMiddleware>();


var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DefaultModelRendering(ModelRendering.Model);
        c.DefaultModelExpandDepth(-1);
        c.DefaultModelsExpandDepth(-1);
    });
}
app.UseMiddleware<RequestSanitizationMiddleware>();
app.UseHttpsRedirection();

app.UseRouting();

app.UseCorsPolicy(builder.Configuration["AllowedHosts"]);

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();


app.UseJwtAuthentication(tokenValidationParameters);
app.Use(async (ctx, next) =>
{

    ctx.Response.Headers.Remove("Server");
    ctx.Response.Headers.Remove("x-powered-by");

    await next();

});

app.MapControllers();
app.Run();
