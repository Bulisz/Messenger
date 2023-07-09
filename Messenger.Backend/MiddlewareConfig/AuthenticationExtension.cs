using Messenger.Backend.DataBase;
using Messenger.Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Messenger.Backend.MiddlewareConfig;

public static class AuthenticationExtension
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {

        services.AddIdentityCore<ApplicationUser>(options =>
                        {
                            options.User.RequireUniqueEmail = true;
                            options.Password.RequireDigit = true;
                            options.Password.RequiredLength = 6;
                            options.Password.RequireNonAlphanumeric = false;
                            options.Password.RequireUppercase = false;
                            options.Password.RequireLowercase = false;
                            options.SignIn.RequireConfirmedEmail = false;
                        })
                 .AddRoles<IdentityRole>()
                 .AddEntityFrameworkStores<AppDbContext>()
                 .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidAudience = configuration["JwtTokensOptions:AccessTokenOptions:Audience"],
                         ValidIssuer = configuration["JwtTokensOptions:AccessTokenOptions:Issuer"],
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                                configuration["JwtTokensOptions:AccessTokenOptions:Key"]
                                 ?? throw new InvalidOperationException("Access token key is missing."))
                         )
                     };
                     options.Events = new JwtBearerEvents
                     {
                         OnMessageReceived = context =>
                         {
                             var accessToken = context.Request.Query["access_token"];

                             // If the request is for our hub...
                             var path = context.HttpContext.Request.Path;
                             if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/messengerhub")))
                             {
                                 // Read the token out of the query string
                                 context.Token = accessToken;
                             }
                             return Task.CompletedTask;
                         }
                     };
                 });

        services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromMinutes(15));

        return services;
    }

    public static async Task<WebApplication> UseAuthAsync(this WebApplication app)
    {
        using (IServiceScope scope = app.Services.CreateScope())
        {
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                bool roleExist = await roleManager.RoleExistsAsync(role.ToString());
                if (!roleExist)
                    _ = await roleManager.CreateAsync(new IdentityRole(role.ToString()));
            }
        }
        return app;
    }
}
