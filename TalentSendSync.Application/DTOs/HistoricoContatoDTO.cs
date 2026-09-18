using TalentSendSync.Domain.Enums;

namespace TalentSendSync.Application.DTOs;

public class HistoricoContatoDTO
{
    public int HistoricoContatoId { get; set; }

    public DateTime DataContato { get; set; }

    public TipoContatoEnum TipoContato { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public Guid CandidaturaId { get; set; }
}
