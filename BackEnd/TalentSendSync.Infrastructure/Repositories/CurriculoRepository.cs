
using Microsoft.EntityFrameworkCore;
using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Domain.Pagination;
using TalentSendSync.Infrastructure.Context;

namespace TalentSendSync.Infrastructure.Repositories;

public class CurriculoRepository : ICurriculoRepository
{

    private readonly AppDbContext _context;

    public CurriculoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Curriculo> CreateAsync(Curriculo curriculo)
    {
        ArgumentNullException.ThrowIfNull(curriculo);

        await _context.Curriculos.AddAsync(curriculo);
        return curriculo;
    }

    public async Task<Curriculo?> GetByIdAsync(Guid? id, string userId)
    {
        if (!id.HasValue || id.Value == Guid.Empty)
            return null;

        return await _context.Curriculos
            .AsNoTracking()
            .Include(curriculo => curriculo.Candidaturas)
            .FirstOrDefaultAsync(curriculo => curriculo.CurriculoId == id.Value && curriculo.UserId == userId);
    }

    public async Task<PagedList<Curriculo>> GetCurriculosPagedAsync(int pageNumber, int pageSize, string userId)
    {
        ValidatePagination(pageNumber, pageSize);

        var query = _context.Curriculos
            .AsNoTracking()
            .Include(curriculo => curriculo.Candidaturas)
            .Where(curriculo => curriculo.UserId == userId)
            .OrderByDescending(curriculo => curriculo.DataCriacao);

        return await PagedList<Curriculo>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public Task<Curriculo> RemoveAsync(Curriculo curriculo)
    {
        ArgumentNullException.ThrowIfNull(curriculo);

        _context.Curriculos.Remove(curriculo);
        return Task.FromResult(curriculo);
    }

    public Task<Curriculo> UpdateAsync(Curriculo curriculo)
    {
        ArgumentNullException.ThrowIfNull(curriculo);

        _context.Curriculos.Update(curriculo);
        return Task.FromResult(curriculo);
    }

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "A página deve ser maior que zero.");

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "O tamanho da página deve ser maior que zero.");
    }
}
