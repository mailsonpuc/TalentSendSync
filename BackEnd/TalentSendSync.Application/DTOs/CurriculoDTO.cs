
namespace TalentSendSync.Application.DTOs;

public class CurriculoDTO
{
    public Guid CurriculoId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string NomeArquivo { get; set; } = string.Empty;

    public string StorageKey { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long TamanhoBytes { get; set; }

    public int Versao { get; set; }

    public DateTime DataCriacao { get; set; }

    public bool Ativo { get; set; }
}
