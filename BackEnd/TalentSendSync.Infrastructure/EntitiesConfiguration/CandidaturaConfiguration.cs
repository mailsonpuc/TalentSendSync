using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSendSync.Domain.Entities;

namespace TalentSendSync.Infrastructure.EntitiesConfiguration;

public class CandidaturaConfiguration: IEntityTypeConfiguration<Candidatura>
{
    public void Configure(EntityTypeBuilder<Candidatura> builder)
    {
        builder.ToTable("Candidaturas"); //pacote Microsoft.EntityFrameworkCore.Relational

        builder.HasKey(candidatura => candidatura.CandidaturaId);

        builder.Property(candidatura => candidatura.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(candidatura => candidatura.UserId);

        builder.Property(candidatura => candidatura.Empresa)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(candidatura => candidatura.Cargo)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(candidatura => candidatura.DataEnvio)
            .IsRequired();

        builder.Property(candidatura => candidatura.PretensaoSalarial)
            .HasPrecision(18, 2);

        builder.Property(candidatura => candidatura.LinkVaga)
            .HasMaxLength(500);

        builder.Property(candidatura => candidatura.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne(candidatura => candidatura.Curriculo)
            .WithMany(curriculo => curriculo.Candidaturas)
            .HasForeignKey(candidatura => candidatura.CurriculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(candidatura => candidatura.HistoricosContato)
            .WithOne(historico => historico.Candidatura)
            .HasForeignKey(historico => historico.CandidaturaId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    
}