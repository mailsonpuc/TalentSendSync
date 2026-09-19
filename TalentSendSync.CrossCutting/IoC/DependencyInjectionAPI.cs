using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Application.Services;
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
            return new CurriculoService(unitOfWork.CurriculoRepository);
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