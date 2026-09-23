

using Microsoft.EntityFrameworkCore;
using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Domain.Pagination;
using TalentSendSync.Infrastructure.Context;

namespace TalentSendSync.Infrastructure.Repositories;

public class CandidaturaRepository : ICandidaturaRepository
{
    private readonly AppDbContext _context;

    public CandidaturaRepository(AppDbContext context)
    {
        _context = context;
    }



    public Task<IQueryable<Candidatura>> GetCandidaturasAsync(string userId)
    {
        IQueryable<Candidatura> query = _context.Candidaturas
            .AsNoTracking()
            .Include(candidatura => candidatura.Curriculo)
            .Include(candidatura => candidatura.HistoricosContato)
            .Where(candidatura => candidatura.UserId == userId);

        return Task.FromResult(query);
    }


 
    public async Task<PagedList<Candidatura>> GetCandidaturasPagedAsync(int pageNumber, int pageSize, string userId)
    {
        ValidatePagination(pageNumber, pageSize);

        var query = _context.Candidaturas
            .AsNoTracking()
            .Include(candidatura => candidatura.Curriculo)
            .Include(candidatura => candidatura.HistoricosContato)
            .Where(candidatura => candidatura.UserId == userId)
            .OrderByDescending(candidatura => candidatura.DataEnvio);

        return await PagedList<Candidatura>.ToPagedListAsync(query, pageNumber, pageSize);
    }


    public async Task<Candidatura> CreateAsync(Candidatura candidatura)
    {
        ArgumentNullException.ThrowIfNull(candidatura);

        await _context.Candidaturas.AddAsync(candidatura);
        return candidatura;
    }


    public async Task<Candidatura?> GetByIdAsync(Guid? id, string userId)
    {
        if (!id.HasValue || id.Value == Guid.Empty)
            return null;

        return await _context.Candidaturas
            .AsNoTracking()
            .Include(candidatura => candidatura.Curriculo)
            .Include(candidatura => candidatura.HistoricosContato)
            .FirstOrDefaultAsync(candidatura => candidatura.CandidaturaId == id.Value && candidatura.UserId == userId);
    }



    public Task<Candidatura> UpdateAsync(Candidatura candidatura)
    {
        ArgumentNullException.ThrowIfNull(candidatura);

        _context.Candidaturas.Update(candidatura);
        return Task.FromResult(candidatura);
    }



    public Task<Candidatura> RemoveAsync(Candidatura candidatura)
    {
        ArgumentNullException.ThrowIfNull(candidatura);

        _context.Candidaturas.Remove(candidatura);
        return Task.FromResult(candidatura);
    }

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "A página deve ser maior que zero.");

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "O tamanho da página deve ser maior que zero.");
    }


}
