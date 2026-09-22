using Microsoft.EntityFrameworkCore;
using TalentSendSync.Domain.Entities;

namespace TalentSendSync.Infrastructure.Context;

public class AppDbContext : DbContext
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
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext)
            .Assembly);
    }
    
}