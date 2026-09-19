using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Infrastructure.Context;
using TalentSendSync.Infrastructure.Repositories;

namespace TalentSendSync.CrossCutting.IoC;

public static class DependencyInjectionAPI
{
    public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services,
        IConfiguration configuration)
    {
        //Usando em Memomy
        //services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("DataBase"));


        //Usando SQL Server
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                // String de conexão vinda do appsettings.json
                configuration.GetConnectionString("DefaultConnection"),

                // Define onde ficarão as migrations
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );



        //Unit Of World
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        return services;

    }

}