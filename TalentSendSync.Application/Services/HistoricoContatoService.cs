using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Application.Mappings;
using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Application.Services;

public class HistoricoContatoService : IHistoricoContatoService
{
    private readonly IHistoricoContatoRepository _historicoContatoRepository;
    private readonly ICandidaturaRepository _candidaturaRepository;

    public HistoricoContatoService(
        IHistoricoContatoRepository historicoContatoRepository,
        ICandidaturaRepository candidaturaRepository)
    {
        _historicoContatoRepository = historicoContatoRepository;
        _candidaturaRepository = candidaturaRepository;
    }

    public async Task<PagedList<HistoricoContatoDTO>> GetHistoricosPagedAsync(int pageNumber, int pageSize)
    {
        var historicos = await _historicoContatoRepository.GetHistoricosPagedAsync(pageNumber, pageSize);
        var historicoDtos = historicos
            .Select(historico => historico.ToHistoricoContatoDTO()!)
            .ToList();

        return new PagedList<HistoricoContatoDTO>(
            historicoDtos,
            historicos.TotalCount,
            historicos.CurrentPage,
            historicos.PageSize);
    }

    public async Task<HistoricoContatoDTO?> GetByIdAsync(Guid? id)
    {
        var historico = await _historicoContatoRepository.GetByIdAsync(id);
        return historico?.ToHistoricoContatoDTO();
    }

    public async Task<HistoricoContatoDTO> CreateAsync(HistoricoContatoDTO historicoContato)
    {
        ArgumentNullException.ThrowIfNull(historicoContato);
        await EnsureCandidaturaExistsAsync(historicoContato.CandidaturaId);

        var historicoEntity = historicoContato.ToHistoricoContato()!;
        var createdHistorico = await _historicoContatoRepository.CreateAsync(historicoEntity);

        return createdHistorico.ToHistoricoContatoDTO()!;
    }

    public async Task<HistoricoContatoDTO> UpdateAsync(HistoricoContatoDTO historicoContato)
    {
        ArgumentNullException.ThrowIfNull(historicoContato);
        await EnsureCandidaturaExistsAsync(historicoContato.CandidaturaId);

        var historicoEntity = await GetEntityByIdAsync(historicoContato.HistoricoContatoId);
        historicoEntity.UpdateDetails(
            historicoContato.DataContato,
            historicoContato.TipoContato,
            historicoContato.Descricao,
            historicoContato.CandidaturaId);

        var updatedHistorico = await _historicoContatoRepository.UpdateAsync(historicoEntity);
        return updatedHistorico.ToHistoricoContatoDTO()!;
    }

    public async Task<HistoricoContatoDTO> RemoveAsync(HistoricoContatoDTO historicoContato)
    {
        ArgumentNullException.ThrowIfNull(historicoContato);

        var historicoEntity = await GetEntityByIdAsync(historicoContato.HistoricoContatoId);
        var removedHistorico = await _historicoContatoRepository.RemoveAsync(historicoEntity);

        return removedHistorico.ToHistoricoContatoDTO()!;
    }

    private async Task<HistoricoContato> GetEntityByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID do histórico é obrigatório.", nameof(id));

        var historico = await _historicoContatoRepository.GetByIdAsync(id);
        return historico ?? throw new KeyNotFoundException("Histórico de contato não encontrado.");
    }

    private async Task EnsureCandidaturaExistsAsync(Guid candidaturaId)
    {
        if (candidaturaId == Guid.Empty)
            throw new ArgumentException("A candidatura é obrigatória.", nameof(candidaturaId));

        if (await _candidaturaRepository.GetByIdAsync(candidaturaId) is null)
            throw new KeyNotFoundException("Candidatura não encontrada.");
    }
}