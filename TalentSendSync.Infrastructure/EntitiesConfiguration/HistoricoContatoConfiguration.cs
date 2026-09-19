using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSendSync.Domain.Entities;

namespace TalentSendSync.Infrastructure.EntitiesConfiguration;

public class HistoricoContatoConfiguration: IEntityTypeConfiguration<HistoricoContato>
{
    public void Configure(EntityTypeBuilder<HistoricoContato> builder)
    {
        builder.ToTable("HistoricosContato");

        builder.HasKey(historico => historico.HistoricoContatoId);

        builder.Property(historico => historico.DataContato)
            .IsRequired();

        builder.Property(historico => historico.TipoContato)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(historico => historico.Descricao)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(historico => historico.Candidatura)
            .WithMany(candidatura => candidatura.HistoricosContato)
            .HasForeignKey(historico => historico.CandidaturaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}