using Microsoft.AspNetCore.Mvc;
using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Domain.Interfaces;

namespace TalentSendSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurriculosController : ControllerBase
{
    private readonly ICurriculoService _curriculoService;
    private readonly IUnitOfWork _unitOfWork;

    public CurriculosController(
        ICurriculoService curriculoService,
        IUnitOfWork unitOfWork)
    {
        _curriculoService = curriculoService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var curriculos = await _curriculoService.GetCurriculosPagedAsync(pageNumber, pageSize);
        return Ok(curriculos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CurriculoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurriculoDTO>> GetById(Guid id)
    {
        var curriculo = await _curriculoService.GetByIdAsync(id);
        return curriculo is null ? NotFound() : Ok(curriculo);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CurriculoDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<CurriculoDTO>> Create([FromBody] CurriculoDTO curriculo)
    {
        var createdCurriculo = await _curriculoService.CreateAsync(curriculo);
        await _unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = createdCurriculo.CurriculoId }, createdCurriculo);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CurriculoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurriculoDTO>> Update(Guid id, [FromBody] CurriculoDTO curriculo)
    {
        if (id != curriculo.CurriculoId)
            return BadRequest("O ID da rota deve ser igual ao ID do currículo.");

        if (await _curriculoService.GetByIdAsync(id) is null)
            return NotFound();

        var updatedCurriculo = await _curriculoService.UpdateAsync(curriculo);
        await _unitOfWork.CommitAsync();

        return Ok(updatedCurriculo);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var curriculo = await _curriculoService.GetByIdAsync(id);
        if (curriculo is null)
            return NotFound();

        await _curriculoService.RemoveAsync(curriculo);
        await _unitOfWork.CommitAsync();

        return NoContent();
    }
}