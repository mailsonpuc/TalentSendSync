using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Enums;

namespace TalentSendSync.Application.DTOs;

public class CandidaturaDTO
{
    public Guid CandidaturaId { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public string Cargo { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; }

    public decimal? PretensaoSalarial { get; set; }

    public string? LinkVaga { get; set; }

    public StatusEnum Status { get; set; }

    public string StatusNome => Status.GetDisplayName();

    public string? Observacoes { get; set; }

    // FK Curriculo
    public Guid CurriculoId { get; set; }

    // Propriedades adicionais para o frontend
    public string? CurriculoNome { get; set; }

    public IEnumerable<HistoricoContatoResumoDTO> HistoricosContato { get; set; } = [];

    // Alias para compatibilidade com frontend
    public DateTime DataCandidatura => DataEnvio;
}