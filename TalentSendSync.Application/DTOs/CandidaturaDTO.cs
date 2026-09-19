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



    // FK Curriculo
    public Guid CurriculoId { get; set; }
    // navegação: uma candidatura aponta para um currículo
    //public Curriculo? Curriculo { get; private set; }

}