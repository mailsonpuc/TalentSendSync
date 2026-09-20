namespace TalentSendSync.Application.Interfaces;

public interface IArquivoStorage
{
    Task SalvarAsync(
        Stream arquivo,
        string storageKey,
        CancellationToken cancellationToken = default);

    Task<Stream> AbrirAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    Task ExcluirAsync(
        string storageKey,
        CancellationToken cancellationToken = default);
}
