namespace TalentSendSync.Domain.Entities;

public class Curriculo
{
    public Guid CurriculoId { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public string Nome { get; private set; } = string.Empty;

    public string NomeArquivo { get; private set; } = string.Empty;

    public string StorageKey { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long TamanhoBytes { get; private set; }

    public int Versao { get; private set; }

    public DateTime DataCriacao { get; private set; }

    public bool Ativo { get; private set; }

    //curriculo tem uma coleçao de candidaturas. 1:N
    public ICollection<Candidatura> Candidaturas { get; private set; } = new List<Candidatura>();





    private Curriculo()
    {
    }

    public Curriculo(
        string nome,
        string nomeArquivo,
        string storageKey,
        string contentType,
        long tamanhoBytes,
        int versao,
        string userId = "legacy")
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("O usuário é obrigatório.", nameof(userId));

        ValidateDomain(nome, nomeArquivo, storageKey, contentType, tamanhoBytes, versao);

        CurriculoId = Guid.NewGuid();
        UserId = userId;
        Nome = nome.Trim();
        NomeArquivo = nomeArquivo.Trim();
        StorageKey = storageKey.Trim();
        ContentType = contentType.Trim();
        TamanhoBytes = tamanhoBytes;
        Versao = versao;
        DataCriacao = DateTime.UtcNow;
        Ativo = true;
    }

    public void UpdateDetails(string nome, int versao)
    {
        ValidateDomain(nome, NomeArquivo, StorageKey, ContentType, TamanhoBytes, versao);

        Nome = nome.Trim();
        Versao = versao;
    }

    public void UpdateFile(
        string nomeArquivo,
        string storageKey,
        string contentType,
        long tamanhoBytes)
    {
        ValidateDomain(Nome, nomeArquivo, storageKey, contentType, tamanhoBytes, Versao);

        NomeArquivo = nomeArquivo.Trim();
        StorageKey = storageKey.Trim();
        ContentType = contentType.Trim();
        TamanhoBytes = tamanhoBytes;
    }

    private static void ValidateDomain(
        string nome,
        string nomeArquivo,
        string storageKey,
        string contentType,
        long tamanhoBytes,
        int versao)
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

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("A chave do arquivo é obrigatória.", nameof(storageKey));

        if (!string.Equals(contentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("O arquivo deve ser um PDF.", nameof(contentType));

        if (tamanhoBytes <= 0)
            throw new ArgumentException("O tamanho do arquivo deve ser maior que zero.", nameof(tamanhoBytes));

        if (versao <= 0)
            throw new ArgumentException(
                "A versão deve ser maior que zero.",
                nameof(versao));
    }
}