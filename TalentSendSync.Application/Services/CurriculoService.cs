using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Application.Mappings;
using TalentSendSync.Domain.Entities;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.Domain.Pagination;

namespace TalentSendSync.Application.Services;

public class CurriculoService : ICurriculoService
{
    private readonly ICurriculoRepository _curriculoRepository;

    public CurriculoService(ICurriculoRepository curriculoRepository)
    {
        _curriculoRepository = curriculoRepository;
    }

    public async Task<PagedList<CurriculoDTO>> GetCurriculosPagedAsync(int pageNumber, int pageSize)
    {
        var curriculos = await _curriculoRepository.GetCurriculosPagedAsync(pageNumber, pageSize);
        var curriculoDtos = curriculos
            .Select(curriculo => curriculo.ToCurriculoDTO()!)
            .ToList();

        return new PagedList<CurriculoDTO>(
            curriculoDtos,
            curriculos.TotalCount,
            curriculos.CurrentPage,
            curriculos.PageSize);
    }

    public async Task<CurriculoDTO?> GetByIdAsync(Guid? id)
    {
        var curriculo = await _curriculoRepository.GetByIdAsync(id);
        return curriculo?.ToCurriculoDTO();
    }

    public async Task<CurriculoDTO> CreateAsync(CurriculoDTO curriculo)
    {
        ArgumentNullException.ThrowIfNull(curriculo);

        var curriculoEntity = curriculo.ToCurriculo()!;
        var createdCurriculo = await _curriculoRepository.CreateAsync(curriculoEntity);

        return createdCurriculo.ToCurriculoDTO()!;
    }

    public async Task<CurriculoDTO> UpdateAsync(CurriculoDTO curriculo)
    {
        ArgumentNullException.ThrowIfNull(curriculo);

        var curriculoEntity = await GetEntityByIdAsync(curriculo.CurriculoId);
        curriculoEntity.UpdateDetails(
            curriculo.Nome,
            curriculo.NomeArquivo,
            curriculo.UrlArquivo,
            curriculo.Versao);

        var updatedCurriculo = await _curriculoRepository.UpdateAsync(curriculoEntity);
        return updatedCurriculo.ToCurriculoDTO()!;
    }

    public async Task<CurriculoDTO> RemoveAsync(CurriculoDTO curriculo)
    {
        ArgumentNullException.ThrowIfNull(curriculo);

        var curriculoEntity = await GetEntityByIdAsync(curriculo.CurriculoId);
        var removedCurriculo = await _curriculoRepository.RemoveAsync(curriculoEntity);

        return removedCurriculo.ToCurriculoDTO()!;
    }

    private async Task<Curriculo> GetEntityByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID do currículo é obrigatório.", nameof(id));

        var curriculo = await _curriculoRepository.GetByIdAsync(id);
        return curriculo ?? throw new KeyNotFoundException("Currículo não encontrado.");
    }
}