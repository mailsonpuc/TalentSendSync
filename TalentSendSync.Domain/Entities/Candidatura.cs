using TalentSendSync.Domain.Enums;

namespace TalentSendSync.Domain.Entities;

public class Candidatura
{
    public Guid CandidaturaId { get; private set; }

    public string Empresa { get; private set; } = string.Empty;

    public string Cargo { get; private set; } = string.Empty;

    public DateTime DataEnvio { get; private set; }

    public decimal? PretensaoSalarial { get; private set; }

    public string? LinkVaga { get; private set; }

    public StatusEnum Status { get; private set; }


    //candidatura tem uma coleçao de historico. 1:N
    public ICollection<HistoricoContato> HistoricosContato { get; private set; } = new List<HistoricoContato>();

    // FK Curriculo
    public Guid CurriculoId { get; private set; }

    // navegação: uma candidatura aponta para um currículo
    public Curriculo? Curriculo { get; private set; }






    public Candidatura(string empresa, string cargo, decimal? pretensaoSalarial, string? linkVaga, StatusEnum status)
    {
        ValidateDomain(empresa, cargo, pretensaoSalarial, linkVaga, status);

        CandidaturaId = Guid.NewGuid();
        Empresa = empresa.Trim();
        Cargo = cargo.Trim();
        DataEnvio = DateTime.UtcNow;
        PretensaoSalarial = pretensaoSalarial;
        LinkVaga = linkVaga?.Trim();
        Status = status;
    }

    private static void ValidateDomain(string empresa, string cargo, decimal? pretensaoSalarial, string? linkVaga, StatusEnum status)
    {
        if (string.IsNullOrWhiteSpace(empresa))
            throw new ArgumentException(
                "Empresa é obrigatória.",
                nameof(empresa));

        if (empresa.Length > 200)
            throw new ArgumentException(
                "Empresa deve possuir no máximo 200 caracteres.",
                nameof(empresa));

        if (string.IsNullOrWhiteSpace(cargo))
            throw new ArgumentException(
                "Cargo é obrigatório.",
                nameof(cargo));

        if (cargo.Length > 150)
            throw new ArgumentException(
                "Cargo deve possuir no máximo 150 caracteres.",
                nameof(cargo));

        if (pretensaoSalarial.HasValue &&
            pretensaoSalarial.Value < 0)
        {
            throw new ArgumentException(
                "Pretensão salarial não pode ser negativa.",
                nameof(pretensaoSalarial));
        }

        if (!string.IsNullOrWhiteSpace(linkVaga) &&
            !Uri.IsWellFormedUriString(linkVaga, UriKind.Absolute))
        {
            throw new ArgumentException(
                "Link da vaga não é uma URL válida.",
                nameof(linkVaga));
        }

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException(
                "Status da candidatura inválido.",
                nameof(status));
        }
    }
}