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
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly LinkGenerator _linkGenerator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService service, LinkGenerator linkGenerator, ILogger<UsersController> logger)
    {
        _service = service;
        _linkGenerator = linkGenerator;
        _logger = logger;
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<UserViewModel>>> Search([FromQuery] UserQueryParameters parameters)
    {
        var result = await _service.SearchAsync(parameters);
        var items = result.Items.Select(ToViewModel).ToList();
        return Ok(new PagedResponse<UserViewModel>(items, result.Page, result.PageSize, result.Total));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserViewModel>> GetById(Guid id)
    {
        var user = await _service.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(ToViewModel(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserViewModel>> Create(UserCreateDto dto)
    {
        var user = await _service.CreateAsync(dto);
        _logger.LogInformation("Usuário {UserId} criado", user.Id);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, ToViewModel(user));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserViewModel>> Update(Guid id, UserUpdateDto dto)
    {
        var user = await _service.UpdateAsync(id, dto);
        _logger.LogInformation("Usuário {UserId} atualizado", user.Id);
        return Ok(ToViewModel(user));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        _logger.LogInformation("Usuário {UserId} removido", id);
        return NoContent();
    }

    private UserViewModel ToViewModel(User user)
    {
        var uri = _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), values: new { id = user.Id }) ?? string.Empty;
        var searchUri = _linkGenerator.GetUriByAction(HttpContext, "Search", "FocusSessions", values: new { }) ?? string.Empty;
        var links = new List<LinkDto>
        {
            new LinkDto(uri, "self", "GET"),
            new LinkDto(uri, "update-user", "PUT"),
            new LinkDto(uri, "delete-user", "DELETE"),
            new LinkDto(searchUri, "focus-sessions", "GET")
        };

        return new UserViewModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Links = links
        };
    }
}

