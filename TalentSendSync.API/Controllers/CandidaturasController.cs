using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Domain.Interfaces;

namespace TalentSendSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidaturasController : ControllerBase
{
	private readonly ICandidaturaService _candidaturaService;
	private readonly IUnitOfWork _unitOfWork;

	public CandidaturasController(
		ICandidaturaService candidaturaService,
		IUnitOfWork unitOfWork)
	{
		_candidaturaService = candidaturaService;
		_unitOfWork = unitOfWork;
	}

	[HttpGet("oculto")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[ProducesResponseType(typeof(IEnumerable<CandidaturaDTO>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<CandidaturaDTO>>> GetAll()
	{
		var candidaturas = await _candidaturaService.GetCandidaturasAsync();
		return Ok(await candidaturas.ToListAsync());
	}

	[HttpGet("pagination")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
	{
		var candidaturas = await _candidaturaService.GetCandidaturasPagedAsync(pageNumber, pageSize);
		return Ok(candidaturas);
	}

	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(CandidaturaDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CandidaturaDTO>> GetById(Guid id)
	{
		var candidatura = await _candidaturaService.GetByIdAsync(id);

		return candidatura is null ? NotFound(new { message = "Nada encontrado" }) : Ok(candidatura);
	}

	[HttpPost]
	[ProducesResponseType(typeof(CandidaturaDTO), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CandidaturaDTO>> Create([FromBody] CandidaturaDTO candidatura)
	{
		if (candidatura.CurriculoId == Guid.Empty)
			return BadRequest("O CurriculoId é obrigatório.");

		if (await _unitOfWork.CurriculoRepository.GetByIdAsync(candidatura.CurriculoId) is null)
			return NotFound("Currículo não encontrado.");

		var createdCandidatura = await _candidaturaService.CreateAsync(candidatura);
		await _unitOfWork.CommitAsync();

		return CreatedAtAction(nameof(GetById), new { id = createdCandidatura.CandidaturaId }, createdCandidatura);
	}

	[HttpPut("{id:guid}")]
	[ProducesResponseType(typeof(CandidaturaDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CandidaturaDTO>> Update(Guid id, [FromBody] CandidaturaDTO candidatura)
	{
		if (id != candidatura.CandidaturaId)
			return BadRequest("O ID da rota deve ser igual ao ID da candidatura.");

		if (await _candidaturaService.GetByIdAsync(id) is null)
			return NotFound();

		var updatedCandidatura = await _candidaturaService.UpdateAsync(candidatura);
		await _unitOfWork.CommitAsync();

		return Ok(updatedCandidatura);
	}

	[HttpDelete("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete(Guid id)
	{
		var candidatura = await _candidaturaService.GetByIdAsync(id);
		if (candidatura is null)
			return NotFound();

		await _candidaturaService.RemoveAsync(candidatura);
		await _unitOfWork.CommitAsync();

		return NoContent();
	}
}
