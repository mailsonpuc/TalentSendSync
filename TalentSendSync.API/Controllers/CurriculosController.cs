using Microsoft.AspNetCore.Mvc;
using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;
using TalentSendSync.Domain.Interfaces;
using TalentSendSync.API.Requests;

namespace TalentSendSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurriculosController : ControllerBase
{
    private readonly ICurriculoService _curriculoService;
    private readonly IArquivoStorage _arquivoStorage;
    private readonly IUnitOfWork _unitOfWork;

    public CurriculosController(
        ICurriculoService curriculoService,
        IArquivoStorage arquivoStorage,
        IUnitOfWork unitOfWork)
    {
        _curriculoService = curriculoService;
        _arquivoStorage = arquivoStorage;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("pagination")]
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
        return curriculo is null
            ? NotFound(new { message = "Currículo não encontrado." })
            : Ok(curriculo);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [ProducesResponseType(typeof(CurriculoDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<CurriculoDTO>> Create(
        [FromForm] CriarCurriculoRequest request)
    {
        if (request.Arquivo is null)
            return BadRequest("O arquivo PDF é obrigatório.");

        await using var arquivoStream = request.Arquivo.OpenReadStream();
        CurriculoDTO createdCurriculo;
        try
        {
            createdCurriculo = await _curriculoService.CreateAsync(
                new CriarCurriculoInput(
                    request.Nome,
                    request.Versao,
                    arquivoStream,
                    request.Arquivo.FileName,
                    request.Arquivo.ContentType,
                    request.Arquivo.Length));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }

        await _unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = createdCurriculo.CurriculoId }, createdCurriculo);
    }

    [HttpGet("{id:guid}/arquivo")]
    [Produces("application/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(Guid id)
    {
        var curriculo = await _curriculoService.GetByIdAsync(id);
        if (curriculo is null)
            return NotFound();

        try
        {
            var arquivo = await _arquivoStorage.AbrirAsync(curriculo.StorageKey);
            return File(arquivo, curriculo.ContentType, curriculo.NomeArquivo);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [ProducesResponseType(typeof(CurriculoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurriculoDTO>> Update(
        Guid id,
        [FromForm] AtualizarCurriculoRequest request)
    {
        var curriculo = await _curriculoService.GetByIdAsync(id);
        if (curriculo is null)
            return NotFound();

        CriarCurriculoInput? novoArquivo = null;
        if (request.Arquivo is not null)
        {
            await using var arquivoStream = request.Arquivo.OpenReadStream();
            novoArquivo = new CriarCurriculoInput(
                request.Nome,
                request.Versao,
                arquivoStream,
                request.Arquivo.FileName,
                request.Arquivo.ContentType,
                request.Arquivo.Length);

            try
            {
                var updatedWithFile = await _curriculoService.UpdateAsync(curriculo, novoArquivo);
                await _unitOfWork.CommitAsync();
                return Ok(updatedWithFile);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        CurriculoDTO updatedCurriculo;
        try
        {
            curriculo.Nome = request.Nome;
            curriculo.Versao = request.Versao;
            updatedCurriculo = await _curriculoService.UpdateAsync(curriculo);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }

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