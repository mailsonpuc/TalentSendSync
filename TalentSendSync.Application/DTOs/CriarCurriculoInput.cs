namespace TalentSendSync.Application.DTOs;

public sealed record CriarCurriculoInput(
    string Nome,
    int Versao,
    Stream Arquivo,
    string NomeArquivo,
    string ContentType,
    long TamanhoBytes);
