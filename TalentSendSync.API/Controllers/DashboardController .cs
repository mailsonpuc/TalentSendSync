using Microsoft.AspNetCore.Mvc;
using TalentSendSync.Application.DTOs;
using TalentSendSync.Application.Interfaces;

namespace TalentSendSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("Candidaturas")]
    [ProducesResponseType(typeof(DashboardDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDTO>> GetCandidaturas()
    {
        return Ok(await _dashboardService.GetCandidaturasAsync());
    }
}