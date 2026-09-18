
namespace TalentSendSync.Application.DTOs;

public class CurriculoDTO
{
    public Guid CurriculoId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string NomeArquivo { get; set; } = string.Empty;

    public string UrlArquivo { get; set; } = string.Empty;

    public int Versao { get; set; }

    public DateTime DataCriacao { get; set; }

    public bool Ativo { get; set; }
}
