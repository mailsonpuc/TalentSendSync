using TalentSendSync.Application.Interfaces;

namespace TalentSendSync.Infrastructure.Storage;

public sealed class LocalFileStorage : IArquivoStorage
{
    private readonly string _directory;

    public LocalFileStorage(string contentRootPath)
    {
        _directory = Path.Combine(contentRootPath, "wwwroot", "doc");
    }

    public async Task SalvarAsync(
        Stream arquivo,
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var caminho = ObterCaminho(storageKey);
        Directory.CreateDirectory(_directory);

        await using var destino = new FileStream(
            caminho,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await arquivo.CopyToAsync(destino, cancellationToken);
    }

    public Task<Stream> AbrirAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var caminho = ObterCaminho(storageKey);
        Stream arquivo = new FileStream(caminho, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(arquivo);
    }

    public Task ExcluirAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var caminho = ObterCaminho(storageKey);
        if (File.Exists(caminho))
            File.Delete(caminho);

        return Task.CompletedTask;
    }

    private string ObterCaminho(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey) ||
            Path.GetFileName(storageKey) != storageKey)
        {
            throw new ArgumentException("Chave de armazenamento inválida.", nameof(storageKey));
        }

        return Path.Combine(_directory, storageKey);
    }
}
