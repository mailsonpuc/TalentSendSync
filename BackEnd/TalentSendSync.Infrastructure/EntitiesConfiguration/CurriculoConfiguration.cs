using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSendSync.Domain.Entities;

namespace TalentSendSync.Infrastructure.EntitiesConfiguration;

public class CurriculoConfiguration : IEntityTypeConfiguration<Curriculo>
{
    public void Configure(EntityTypeBuilder<Curriculo> builder)
    {
        builder.ToTable("Curriculos");

        builder.HasKey(curriculo => curriculo.CurriculoId);

        builder.Property(curriculo => curriculo.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(curriculo => curriculo.UserId);

        builder.Property(curriculo => curriculo.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(curriculo => curriculo.NomeArquivo)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(curriculo => curriculo.StorageKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(curriculo => curriculo.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(curriculo => curriculo.TamanhoBytes)
            .IsRequired();

        builder.Property(curriculo => curriculo.Versao)
            .IsRequired();

        builder.Property(curriculo => curriculo.DataCriacao)
            .IsRequired();

        builder.Property(curriculo => curriculo.Ativo)
            .IsRequired();
    }
    
}