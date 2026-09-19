namespace TalentSendSync.Domain.Entities;

public class Curriculo
{
    public Guid CurriculoId { get; private set; }

    public string Nome { get; private set; } = string.Empty;

    public string NomeArquivo { get; private set; } = string.Empty;

    public string UrlArquivo { get; private set; } = string.Empty;

    public int Versao { get; private set; }

    public DateTime DataCriacao { get; private set; }

    public bool Ativo { get; private set; }

    //curriculo tem uma coleçao de candidaturas. 1:N
    public ICollection<Candidatura> Candidaturas { get; private set; } = new List<Candidatura>();





    private Curriculo()
    {
    }

    public Curriculo(string nome, string nomeArquivo, string urlArquivo, int versao)
    {
        ValidateDomain(nome, nomeArquivo, urlArquivo, versao);

        CurriculoId = Guid.NewGuid();
        Nome = nome.Trim();
        NomeArquivo = nomeArquivo.Trim();
        UrlArquivo = urlArquivo.Trim();
        Versao = versao;
        DataCriacao = DateTime.UtcNow;
        Ativo = true;
    }

    public void UpdateDetails(string nome, string nomeArquivo, string urlArquivo, int versao)
    {
        ValidateDomain(nome, nomeArquivo, urlArquivo, versao);

        Nome = nome.Trim();
        NomeArquivo = nomeArquivo.Trim();
        UrlArquivo = urlArquivo.Trim();
        Versao = versao;
    }

    private static void ValidateDomain(string nome, string nomeArquivo, string urlArquivo, int versao)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException(
                "Nome do currículo é obrigatório.",
                nameof(nome));

        if (nome.Length > 150)
            throw new ArgumentException(
                "Nome do currículo deve possuir no máximo 150 caracteres.",
                nameof(nome));

        if (string.IsNullOrWhiteSpace(nomeArquivo))
            throw new ArgumentException(
                "Nome do arquivo é obrigatório.",
                nameof(nomeArquivo));

        if (nomeArquivo.Length > 255)
            throw new ArgumentException(
                "Nome do arquivo deve possuir no máximo 255 caracteres.",
                nameof(nomeArquivo));

        if (string.IsNullOrWhiteSpace(urlArquivo))
            throw new ArgumentException(
                "URL do arquivo é obrigatória.",
                nameof(urlArquivo));

        if (!Uri.IsWellFormedUriString(urlArquivo, UriKind.Absolute))
            throw new ArgumentException(
                "URL do arquivo não é válida.",
                nameof(urlArquivo));

        if (versao <= 0)
            throw new ArgumentException(
                "A versão deve ser maior que zero.",
                nameof(versao));
    }
}