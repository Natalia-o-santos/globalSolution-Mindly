using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Mindly.Api.DTOs.Application;
using Mindly.Api.Domain.Entities;
using Mindly.Api.Services;

namespace Mindly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FocusSessionsController : ControllerBase
{
    private readonly IFocusSessionService _service;
    private readonly LinkGenerator _linkGenerator;
    private readonly ILogger<FocusSessionsController> _logger;

    public FocusSessionsController(IFocusSessionService service, LinkGenerator linkGenerator, ILogger<FocusSessionsController> logger)
    {
        _service = service;
        _linkGenerator = linkGenerator;
        _logger = logger;
    }

    [HttpGet("search" )]
    public async Task<ActionResult<PagedResponse<FocusSessionViewModel>>> Search([FromQuery] FocusSessionQueryParameters parameters)
    {
        var result = await _service.SearchAsync(parameters);
        var items = result.Items.Select(ToViewModel).ToList();
        return Ok(new PagedResponse<FocusSessionViewModel>(items, result.Page, result.PageSize, result.Total));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FocusSessionViewModel>> GetById(Guid id)
    {
        var session = await _service.GetByIdAsync(id);
        if (session is null)
        {
            return NotFound();
        }

        return Ok(ToViewModel(session));
    }

    [HttpPost]
    public async Task<ActionResult<FocusSessionViewModel>> Create(FocusSessionCreateDto dto)
    {
        var session = await _service.CreateAsync(dto);
        _logger.LogInformation("Sessão {SessionId} criada", session.Id);
        return CreatedAtAction(nameof(GetById), new { id = session.Id }, ToViewModel(session));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FocusSessionViewModel>> Update(Guid id, FocusSessionUpdateDto dto)
    {
        var session = await _service.UpdateAsync(id, dto);
        _logger.LogInformation("Sessão {SessionId} atualizada", session.Id);
        return Ok(ToViewModel(session));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        _logger.LogInformation("Sessão {SessionId} removida", id);
        return NoContent();
    }

    private FocusSessionViewModel ToViewModel(FocusSession session)
    {
        var uri = _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), values: new { id = session.Id }) ?? string.Empty;
        var links = new List<LinkDto>
        {
            new LinkDto(uri, "self", "GET"),
            new LinkDto(uri, "update-session", "PUT"),
            new LinkDto(uri, "delete-session", "DELETE")
        };

        return new FocusSessionViewModel
        {
            Id = session.Id,
            Title = session.Title,
            Description = session.Description,
            FocusMinutes = session.FocusMinutes,
            BreakMinutes = session.BreakMinutes,
            Status = session.Status,
            IoTIntegrationEnabled = session.IoTIntegrationEnabled,
            UserId = session.UserId,
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
            Links = links
        };
    }
}
