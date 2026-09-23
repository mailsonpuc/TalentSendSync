using TalentSendSync.Domain.Enums;

namespace TalentSendSync.Domain.Entities;

public class Candidatura
{
    public Guid CandidaturaId { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public string Empresa { get; private set; } = string.Empty;

    public string Cargo { get; private set; } = string.Empty;

    public DateTime DataEnvio { get; private set; }

    public decimal? PretensaoSalarial { get; private set; }

    public string? LinkVaga { get; private set; }

    public StatusEnum Status { get; private set; }

    public string? Observacoes { get; private set; }

    //candidatura tem uma coleçao de historico. 1:N
    public ICollection<HistoricoContato> HistoricosContato { get; private set; } = new List<HistoricoContato>();

    // FK Curriculo
    public Guid CurriculoId { get; private set; }

    // navegação: uma candidatura aponta para um currículo
    public Curriculo? Curriculo { get; private set; }

    public Candidatura(string empresa, string cargo, decimal? pretensaoSalarial, string? linkVaga, StatusEnum status)
        : this(empresa, cargo, pretensaoSalarial, linkVaga, status, Guid.Empty, null, "legacy")
    {
    }

    public Candidatura(
        string empresa,
        string cargo,
        decimal? pretensaoSalarial,
        string? linkVaga,
        StatusEnum status,
        Guid curriculoId,
        string? observacoes,
        string userId = "legacy")
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("O usuário é obrigatório.", nameof(userId));

        ValidateDomain(empresa, cargo, pretensaoSalarial, linkVaga, status);
        if (curriculoId != Guid.Empty)
            ValidateCurriculoId(curriculoId);

        CandidaturaId = Guid.NewGuid();
        UserId = userId;
        Empresa = empresa.Trim();
        Cargo = cargo.Trim();
        DataEnvio = DateTime.UtcNow;
        PretensaoSalarial = pretensaoSalarial;
        LinkVaga = linkVaga?.Trim();
        Status = status;
        Observacoes = observacoes?.Trim();
        CurriculoId = curriculoId;
    }

    public void UpdateDetails(string empresa, string cargo, decimal? pretensaoSalarial, string? linkVaga, StatusEnum status, string? observacoes = null)
    {
        ValidateDomain(empresa, cargo, pretensaoSalarial, linkVaga, status);

        Empresa = empresa.Trim();
        Cargo = cargo.Trim();
        PretensaoSalarial = pretensaoSalarial;
        LinkVaga = linkVaga?.Trim();
        Status = status;
        Observacoes = observacoes?.Trim();
    }

    public void UpdateObservacoes(string? observacoes)
    {
        Observacoes = observacoes?.Trim();
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

    private static void ValidateCurriculoId(Guid curriculoId)
    {
        if (curriculoId == Guid.Empty)
            throw new ArgumentException("O currículo é obrigatório.", nameof(curriculoId));
    }
}