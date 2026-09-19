

namespace TalentSendSync.Domain.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    ICandidaturaRepository CandidaturaRepository { get; }
    ICurriculoRepository CurriculoRepository { get; }
    IHistoricoContatoRepository HistoricoContatoRepository { get; }


    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    
}
