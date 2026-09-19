using Microsoft.EntityFrameworkCore;
using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Domain.Pagination;
using TalentSendSync.Infrastructure.Context;

namespace TalentSendSync.Infrastructure.Repositories;

public class HistoricoContatoRepository : IHistoricoContatoRepository
{

    private readonly AppDbContext _context;

    public HistoricoContatoRepository(AppDbContext context)
    {
        _context = context;
    }



    public async Task<HistoricoContato> CreateAsync(HistoricoContato historicoContato)
    {
        ArgumentNullException.ThrowIfNull(historicoContato);

        await _context.HistoricosContato.AddAsync(historicoContato);
        return historicoContato;
    }

    public async Task<HistoricoContato?> GetByIdAsync(Guid? id)
    {
        if (!id.HasValue || id.Value == Guid.Empty)
            return null;

        return await _context.HistoricosContato
            .AsTracking()
            .Include(historico => historico.Candidatura)
                .ThenInclude(candidatura => candidatura!.Curriculo)
            .FirstOrDefaultAsync(historico => historico.HistoricoContatoId == id.Value);
    }

    public async Task<PagedList<HistoricoContato>> GetHistoricosPagedAsync(int pageNumber, int pageSize)
    {
        ValidatePagination(pageNumber, pageSize);

        var query = _context.HistoricosContato
            .AsTracking()
            .Include(historico => historico.Candidatura)
                .ThenInclude(candidatura => candidatura!.Curriculo)
            .OrderByDescending(historico => historico.DataContato);

        return await PagedList<HistoricoContato>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public Task<HistoricoContato> RemoveAsync(HistoricoContato historicoContato)
    {
        ArgumentNullException.ThrowIfNull(historicoContato);

        _context.HistoricosContato.Remove(historicoContato);
        return Task.FromResult(historicoContato);
    }

    public Task<HistoricoContato> UpdateAsync(HistoricoContato historicoContato)
    {
        ArgumentNullException.ThrowIfNull(historicoContato);

        _context.HistoricosContato.Update(historicoContato);
        return Task.FromResult(historicoContato);
    }

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "A página deve ser maior que zero.");

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "O tamanho da página deve ser maior que zero.");
    }
}
