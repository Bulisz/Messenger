﻿namespace Messenger.Backend.MiddlewareConfig;

public static class CorsRules
{
    public static IServiceCollection AddCorsRules(this IServiceCollection services)
    {
        services.AddCors(options => options
                    .AddDefaultPolicy(policyConfig => policyConfig
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials()));

        return services;
    }
}
