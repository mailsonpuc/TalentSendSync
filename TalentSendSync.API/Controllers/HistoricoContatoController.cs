
using Microsoft.AspNetCore.Mvc;
using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Domain.Interfaces;

namespace TalentSendSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistoricoContatoController : ControllerBase
{
	private readonly IHistoricoContatoService _historicoContatoService;
	private readonly IUnitOfWork _unitOfWork;

	public HistoricoContatoController(
		IHistoricoContatoService historicoContatoService,
		IUnitOfWork unitOfWork)
	{
		_historicoContatoService = historicoContatoService;
		_unitOfWork = unitOfWork;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult> GetPaged(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var historicos = await _historicoContatoService.GetHistoricosPagedAsync(pageNumber, pageSize);
		return Ok(historicos);
	}

	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(HistoricoContatoDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<HistoricoContatoDTO>> GetById(Guid id)
	{
		var historico = await _historicoContatoService.GetByIdAsync(id);
		return historico is null ? NotFound() : Ok(historico);
	}

	[HttpPost]
	[ProducesResponseType(typeof(HistoricoContatoDTO), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<HistoricoContatoDTO>> Create([FromBody] HistoricoContatoDTO historicoContato)
	{
		if (await _unitOfWork.CandidaturaRepository.GetByIdAsync(historicoContato.CandidaturaId) is null)
			return NotFound("Candidatura não encontrada.");

		var createdHistorico = await _historicoContatoService.CreateAsync(historicoContato);
		await _unitOfWork.CommitAsync();

		return CreatedAtAction(nameof(GetById), new { id = createdHistorico.HistoricoContatoId }, createdHistorico);
	}

	[HttpPut("{id:guid}")]
	[ProducesResponseType(typeof(HistoricoContatoDTO), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<HistoricoContatoDTO>> Update(
		Guid id,
		[FromBody] HistoricoContatoDTO historicoContato)
	{
		if (id != historicoContato.HistoricoContatoId)
			return BadRequest("O ID da rota deve ser igual ao ID do histórico.");

		if (await _historicoContatoService.GetByIdAsync(id) is null)
			return NotFound();

		if (await _unitOfWork.CandidaturaRepository.GetByIdAsync(historicoContato.CandidaturaId) is null)
			return NotFound("Candidatura não encontrada.");

		var updatedHistorico = await _historicoContatoService.UpdateAsync(historicoContato);
		await _unitOfWork.CommitAsync();

		return Ok(updatedHistorico);
	}

	[HttpDelete("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete(Guid id)
	{
		var historico = await _historicoContatoService.GetByIdAsync(id);
		if (historico is null)
			return NotFound();

		await _historicoContatoService.RemoveAsync(historico);
		await _unitOfWork.CommitAsync();

		return NoContent();
	}

}
