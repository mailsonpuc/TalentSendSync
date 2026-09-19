

using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Infrastructure.Context;

namespace TalentSendSync.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ICandidaturaRepository? _candidaturaRepository;
    private ICurriculoRepository? _curriculoRepository;
    private IHistoricoContatoRepository? _historicoContatoRepository;

    
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }



    public ICandidaturaRepository CandidaturaRepository
    {
        get => _candidaturaRepository ??= new CandidaturaRepository(_context);
    }

    public ICurriculoRepository CurriculoRepository
    {
        get => _curriculoRepository ??= new CurriculoRepository(_context);
    }



    public IHistoricoContatoRepository HistoricoContatoRepository
    {
        get => _historicoContatoRepository ??= new HistoricoContatoRepository(_context);
    }


    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }



    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }


}
