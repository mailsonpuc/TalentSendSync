using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Application.Services;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Infrastructure.Context;
using TalentSendSync.Infrastructure.HealthChecks;
using TalentSendSync.Infrastructure.Identity;
using TalentSendSync.Infrastructure.Identity.Validators;
using TalentSendSync.Infrastructure.Identity.Interfaces;
using TalentSendSync.Infrastructure.Identity.Services;
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
        services.AddScoped<IDashboardService>(serviceProvider =>
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new DashboardService(unitOfWork.CandidaturaRepository);
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



        // ===============================
        // CONFIGURAÇÃO DO ASP.NET IDENTITY
        // ===============================
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.RemoveAll<IUserValidator<ApplicationUser>>();
        services.AddScoped<IUserValidator<ApplicationUser>, UserValidator>();



        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        });

              // ===============================
        // SERVIÇO DE AUTENTICAÇÃO
        // ===============================
        services.AddScoped<IAuthenticate, AuthenticateService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;

    }

}