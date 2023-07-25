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

        if (builder.Environment.IsDevelopment())
        {
            string connectionString = builder.Configuration.GetConnectionString("Development") ?? throw new InvalidOperationException("No connectionString");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        }
        else
        {
            string connectionString = builder.Configuration.GetConnectionString("Production") ?? throw new InvalidOperationException("No connectionString");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        }
            

        //builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<AppDbContext>();


        // Add services to the container.

        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRepository,UserRepository>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();

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

        using (var scope = app.Services.CreateScope())
        using (var context = scope.ServiceProvider.GetRequiredService<AppDbContext>())
        {
            await context.Database.MigrateAsync();
        }

        await app.UseAuthAsync();

        app.Run();
    }
}