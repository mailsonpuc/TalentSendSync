using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Application.Services;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Infrastructure.Context;
using TalentSendSync.Infrastructure.HealthChecks;
using TalentSendSync.Infrastructure.Repositories;
using TalentSendSync.Infrastructure.Storage;

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

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");

        services.AddScoped<IArquivoStorage>(serviceProvider =>
            new LocalFileStorage(serviceProvider
                .GetRequiredService<IHostEnvironment>()
                .ContentRootPath));



        //Unit Of World
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICandidaturaService>(serviceProvider =>
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new CandidaturaService(
                unitOfWork.CandidaturaRepository,
                unitOfWork.CurriculoRepository);
        });
        services.AddScoped<ICurriculoService>(serviceProvider =>
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new CurriculoService(
                unitOfWork.CurriculoRepository,
                serviceProvider.GetRequiredService<IArquivoStorage>());
        });
        services.AddScoped<IHistoricoContatoService>(serviceProvider =>
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new HistoricoContatoService(
                unitOfWork.HistoricoContatoRepository,
                unitOfWork.CandidaturaRepository);
        });


        return services;

    }

}