using Microsoft.AspNetCore.Http;

namespace TalentSendSync.API.Requests;

public sealed class CriarCurriculoRequest
{
    public string Nome { get; set; } = string.Empty;

    public int Versao { get; set; }

    public IFormFile Arquivo { get; set; } = default!;
}
