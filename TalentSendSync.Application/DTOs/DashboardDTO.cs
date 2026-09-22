using TalentSendSync.Domain.Enums;

namespace TalentSendSync.Application.DTOs;

public class DashboardDTO
{
    public int TotalCandidaturas { get; set; }
    public int Enviadas { get; set; }
    public int EmAndamento { get; set; }
    public int PropostasRecebidas { get; set; }
    public int Aprovadas { get; set; }
    public int Rejeitadas { get; set; }
    public int Canceladas { get; set; }
    public int SemRetorno { get; set; }
    public decimal TaxaDeResposta { get; set; }
    public decimal TaxaDeAprovacao { get; set; }
    public List<DashboardStatusDTO> Status { get; set; } = [];
    public List<DashboardEvolucaoDTO> Evolucao { get; set; } = [];
    public List<DashboardCandidaturaDTO> UltimasCandidaturas { get; set; } = [];
}

public class DashboardStatusDTO
{
    public string Status { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class DashboardEvolucaoDTO
{
    public string Mes { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class DashboardCandidaturaDTO
{
    public Guid CandidaturaId { get; set; }
    public string Empresa { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; }
    public StatusEnum Status { get; set; }
    public string StatusNome => Status.GetDisplayName();
}