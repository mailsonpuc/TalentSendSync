using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentSendSync.Domain.Entities;
using TalentSendSync.Infrastructure.Identity;

namespace TalentSendSync.Infrastructure.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Candidatura> Candidaturas { get; set; }
    public DbSet<Curriculo> Curriculos { get; set; }
    public DbSet<HistoricoContato> HistoricosContato { get; set; }

    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>()
            .HasIndex(user => user.NormalizedUserName)
            .IsUnique(false)
            .HasFilter("[NormalizedUserName] IS NOT NULL");
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext)
            .Assembly);
    }
    
}