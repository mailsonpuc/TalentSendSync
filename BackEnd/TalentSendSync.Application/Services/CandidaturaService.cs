
using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Application.Mappings;
using TalentSendSync.Domain.Enums;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Application.Services;

public class CandidaturaService : ICandidaturaService
{
    private readonly ICandidaturaRepository _candidaturaRepository;
    private readonly ICurriculoRepository _curriculoRepository;
    private readonly ICurrentUser _currentUser;

    public CandidaturaService(
        ICandidaturaRepository candidaturaRepository,
        ICurriculoRepository curriculoRepository,
        ICurrentUser currentUser)
    {
        _candidaturaRepository = candidaturaRepository;
        _curriculoRepository = curriculoRepository;
        _currentUser = currentUser;
    }



    public async Task<CandidaturaDTO> CreateAsync(CandidaturaCreateDTO candidatura)
    {
        ArgumentNullException.ThrowIfNull(candidatura);
        await EnsureCurriculoExistsAsync(candidatura.CurriculoId);

        var candidaturaEntity = candidatura.ToCandidatura(_currentUser.UserId)!;
        var createdCandidatura = await _candidaturaRepository.CreateAsync(candidaturaEntity);

        return createdCandidatura.ToCandidaturaDTO()!;
    }

    public async Task<CandidaturaDTO?> GetByIdAsync(Guid? id)
    {
        var candidatura = await _candidaturaRepository.GetByIdAsync(id, _currentUser.UserId);
        return candidatura?.ToCandidaturaDTO();
    }

    public async Task<IQueryable<CandidaturaDTO>> GetCandidaturasAsync()
    {
        var candidaturas = await _candidaturaRepository.GetCandidaturasAsync(_currentUser.UserId);
        return candidaturas.Select(candidatura => new CandidaturaDTO
        {
            CandidaturaId = candidatura.CandidaturaId,
            Empresa = candidatura.Empresa,
            Cargo = candidatura.Cargo,
            DataEnvio = candidatura.DataEnvio,
            PretensaoSalarial = candidatura.PretensaoSalarial,
            LinkVaga = candidatura.LinkVaga,
            Status = candidatura.Status,
            CurriculoId = candidatura.CurriculoId,
            Observacoes = candidatura.Observacoes
        });
    }

    public async Task<PagedList<CandidaturaDTO>> GetCandidaturasPagedAsync(int pageNumber, int pageSize)
    {
        var candidaturas = await _candidaturaRepository.GetCandidaturasPagedAsync(pageNumber, pageSize, _currentUser.UserId);
        var candidaturaDtos = candidaturas
            .Select(candidatura => candidatura.ToCandidaturaDTO()!)
            .ToList();

        return new PagedList<CandidaturaDTO>(
            candidaturaDtos,
            candidaturas.TotalCount,
            candidaturas.CurrentPage,
            candidaturas.PageSize);
    }

    public async Task<CandidaturaDTO> RemoveAsync(CandidaturaDTO candidatura)
    {
        ArgumentNullException.ThrowIfNull(candidatura);

        var candidaturaEntity = await GetEntityByIdAsync(candidatura.CandidaturaId);
        var removedCandidatura = await _candidaturaRepository.RemoveAsync(candidaturaEntity);

        return removedCandidatura.ToCandidaturaDTO()!;
    }

    public async Task<CandidaturaDTO> UpdateAsync(CandidaturaDTO candidatura)
    {
        ArgumentNullException.ThrowIfNull(candidatura);

        var candidaturaEntity = await GetEntityByIdAsync(candidatura.CandidaturaId);
        candidaturaEntity.UpdateDetails(
            candidatura.Empresa,
            candidatura.Cargo,
            candidatura.PretensaoSalarial,
            candidatura.LinkVaga,
            candidatura.Status,
            candidatura.Observacoes);

        var updatedCandidatura = await _candidaturaRepository.UpdateAsync(candidaturaEntity);
        return updatedCandidatura.ToCandidaturaDTO()!;
    }

    private async Task<Domain.Entities.Candidatura> GetEntityByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID da candidatura é obrigatório.", nameof(id));

        var candidatura = await _candidaturaRepository.GetByIdAsync(id, _currentUser.UserId);
        return candidatura ?? throw new KeyNotFoundException("Candidatura não encontrada.");
    }

    private async Task EnsureCurriculoExistsAsync(Guid curriculoId)
    {
        if (curriculoId == Guid.Empty)
            throw new ArgumentException("O currículo é obrigatório.", nameof(curriculoId));

        if (await _curriculoRepository.GetByIdAsync(curriculoId, _currentUser.UserId) is null)
            throw new KeyNotFoundException("Currículo não encontrado.");
    }
}
