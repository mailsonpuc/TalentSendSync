using TalentSendSync.Domain.Enums;

namespace TalentSendSync.Domain.Entities;

public class HistoricoContato
{
    public Guid HistoricoContatoId { get; private set; }

    public DateTime DataContato { get; private set; }

    public TipoContatoEnum TipoContato { get; private set; }

    public string Descricao { get; private set; } = string.Empty;


    // FK Candidatura chave primaria
    public Guid CandidaturaId { get; private set; }

    //navegaçao quando carregar Historico tambem carregar tudo de Candidatura associado
    public Candidatura? Candidatura { get; private set; }





    public HistoricoContato(DateTime dataContato, TipoContatoEnum tipoContato, string descricao, Guid candidaturaId)
    {
        ValidateDomain(dataContato, tipoContato, descricao, candidaturaId);

        HistoricoContatoId = Guid.NewGuid();
        DataContato = dataContato;
        TipoContato = tipoContato;
        Descricao = descricao.Trim();
        CandidaturaId = candidaturaId;
    }

    public void UpdateDetails(
        DateTime dataContato,
        TipoContatoEnum tipoContato,
        string descricao,
        Guid candidaturaId)
    {
        ValidateDomain(dataContato, tipoContato, descricao, candidaturaId);

        DataContato = dataContato;
        TipoContato = tipoContato;
        Descricao = descricao.Trim();
        CandidaturaId = candidaturaId;
    }


    private static void ValidateDomain(DateTime dataContato, TipoContatoEnum tipoContato, string descricao, Guid candidaturaId)
    {
        if (dataContato == default)
            throw new ArgumentException(
                "A data do contato é obrigatória.",
                nameof(dataContato));

        if (dataContato > DateTime.UtcNow)
            throw new ArgumentException(
                "A data do contato não pode ser futura.",
                nameof(dataContato));

        if (!Enum.IsDefined(tipoContato))
            throw new ArgumentException(
                "Tipo de contato inválido.",
                nameof(tipoContato));

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException(
                "A descrição do contato é obrigatória.",
                nameof(descricao));

        if (descricao.Length > 1000)
            throw new ArgumentException(
                "A descrição deve possuir no máximo 1000 caracteres.",
                nameof(descricao));

        if (candidaturaId == Guid.Empty)
            throw new ArgumentException(
                "A candidatura é obrigatória.",
                nameof(candidaturaId));
    }
}