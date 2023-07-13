using Google;
using Messenger.Backend.Abstactions;
using Messenger.Backend.DataBase;
using Messenger.Backend.Hubs;
using Messenger.Backend.MiddlewareConfig;
using Messenger.Backend.Models.AuthDTOs;
using Messenger.Backend.Repositories;
using Messenger.Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Backend;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOptions<JwtTokensOptions>()
                        .BindConfiguration(nameof(JwtTokensOptions))
                        .ValidateDataAnnotations();

        string connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("No connectionString");
        builder.Services.AddDbContext <AppDbContext > (options => options.UseSqlServer(connectionString));

        //builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<AppDbContext>();


        // Add services to the container.

        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRepository,UserRepository>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("GoogleAuth"));

        builder.Services.AddCorsRules();
        builder.Services.AddMemoryCache();
        builder.Services.AddIdentity();
        builder.Services.AddAuth(builder.Configuration);
        builder.Services.AddSignalR();

        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.MapFallbackToFile("index.html");
        app.MapHub<MessengerHub>("/messengerhub");

        await app.UseAuthAsync();

        app.Run();
    }
}