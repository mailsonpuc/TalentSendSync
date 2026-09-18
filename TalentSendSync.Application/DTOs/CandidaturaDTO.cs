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

}