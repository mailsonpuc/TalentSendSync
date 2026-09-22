using System.Text.Json.Serialization;
using TalentSendSync.Domain.Enums;

namespace TalentSendSync.Application.DTOs;

public class HistoricoContatoDTO
{
    public Guid HistoricoContatoId { get; set; }

    public DateTime DataContato { get; set; }

    public TipoContatoEnum TipoContato { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public Guid CandidaturaId { get; set; }

    public string? Responsavel { get; set; }

    // Propriedades adicionais para o frontend
    public string? CandidaturaStatus { get; set; }

    public string? CandidaturaStatusNome { get; set; }

    [JsonIgnore]
    public CandidaturaHistoricoDTO? Candidatura { get; set; }
}

public class HistoricoContatoCreateDTO
{
    public DateTime DataContato { get; set; }
    public TipoContatoEnum TipoContato { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Guid CandidaturaId { get; set; }
    public string? Responsavel { get; set; }
}

public class CandidaturaHistoricoDTO
{
    public Guid CandidaturaId { get; set; }
    public string Empresa { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; }
    public decimal? PretensaoSalarial { get; set; }
    public string? LinkVaga { get; set; }
    public Domain.Enums.StatusEnum Status { get; set; }
    public string StatusNome => Status.GetDisplayName();
    public Guid CurriculoId { get; set; }
    public CurriculoDTO? Curriculo { get; set; }
    public IEnumerable<HistoricoContatoResumoDTO> HistoricosContato { get; set; } = [];
}

public class HistoricoContatoResumoDTO
{
    public Guid HistoricoContatoId { get; set; }
    public DateTime DataContato { get; set; }
    public TipoContatoEnum TipoContato { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Guid CandidaturaId { get; set; }
    public string? Responsavel { get; set; }
}