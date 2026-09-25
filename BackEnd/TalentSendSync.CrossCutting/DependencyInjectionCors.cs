using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TalentSendSync.CrossCutting;

public static class DependencyInjectionCors
{

    public static IServiceCollection AddInfrastructureCors(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:5173",                  // front Vite dev server
                        "http://localhost:5079",                  // back HTTP
                        "https://localhost:7014",                 // back HTTPS
                        "https://pet-shoop-full-stack.vercel.app" // front produção
                    )
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization");
            });
        });

        return services;
    }
}
