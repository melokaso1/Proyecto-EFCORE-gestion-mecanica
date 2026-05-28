using System.Security.Claims;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/ordenesservicio/{idOrdenServicio:int}")]
[Authorize]
public class DiagnosticoController(IDiagnosticoService diagnosticoService) : ControllerBase
{
    [HttpGet("diagnostico")]
    [Authorize(Roles = "Admin,Mecánico,JefeMecanicos")]
    public async Task<ActionResult<DiagnosticoDto>> ObtenerDiagnostico(int idOrdenServicio)
    {
        var diag = await diagnosticoService.ObtenerAsync(idOrdenServicio);
        return diag is null ? NotFound() : Ok(diag);
    }

    [HttpPut("diagnostico")]
    [Authorize(Roles = "Mecánico")]
    public async Task<ActionResult<DiagnosticoDto>> UpsertDiagnostico(int idOrdenServicio, [FromBody] UpsertDiagnosticoDto dto)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            var result = await diagnosticoService.UpsertAsync(idOrdenServicio, idUsuario, dto);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("reparaciones")]
    [Authorize(Roles = "Admin,Mecánico,JefeMecanicos")]
    public async Task<ActionResult<IEnumerable<ReparacionItemDto>>> ListarReparaciones(int idOrdenServicio)
    {
        var items = await diagnosticoService.ListarReparacionesAsync(idOrdenServicio);
        return Ok(items);
    }

    [HttpPost("reparaciones")]
    [Authorize(Roles = "Mecánico")]
    public async Task<ActionResult<ReparacionItemDto>> CrearReparacion(int idOrdenServicio, [FromBody] CreateReparacionItemDto dto)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            var item = await diagnosticoService.CrearReparacionAsync(idOrdenServicio, idUsuario, dto);
            return Ok(item);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("reparaciones/{idReparacionItem:int}")]
    [Authorize(Roles = "Mecánico")]
    public async Task<ActionResult<ReparacionItemDto>> ActualizarReparacion(int idOrdenServicio, int idReparacionItem, [FromBody] UpdateReparacionItemDto dto)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            var item = await diagnosticoService.ActualizarReparacionAsync(idOrdenServicio, idReparacionItem, idUsuario, dto);
            return Ok(item);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("reparaciones/{idReparacionItem:int}/decision-jefe")]
    [Authorize(Roles = "JefeMecanicos")]
    public async Task<ActionResult<ReparacionItemDto>> DecisionJefe(int idOrdenServicio, int idReparacionItem, [FromBody] DecisionJefeDto dto)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            var item = await diagnosticoService.DecidirJefeAsync(idOrdenServicio, idReparacionItem, idUsuario, dto);
            return Ok(item);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("reparaciones/{idReparacionItem:int}/iniciar")]
    [Authorize(Roles = "Mecánico")]
    public async Task<ActionResult<ReparacionItemDto>> IniciarReparacion(int idOrdenServicio, int idReparacionItem)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            var item = await diagnosticoService.IniciarReparacionAsync(idOrdenServicio, idReparacionItem, idUsuario);
            return Ok(item);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("reparaciones/{idReparacionItem:int}/terminar")]
    [Authorize(Roles = "Mecánico")]
    public async Task<ActionResult<ReparacionItemDto>> TerminarReparacion(int idOrdenServicio, int idReparacionItem)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            var item = await diagnosticoService.TerminarReparacionAsync(idOrdenServicio, idReparacionItem, idUsuario);
            return Ok(item);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    public sealed class ClienteDecisionesRequest
    {
        public Dictionary<int, DecisionClienteDto> Decisiones { get; set; } = new();
    }

    [HttpPatch("reparaciones/decision-cliente")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<IEnumerable<ReparacionItemDto>>> DecisionCliente(int idOrdenServicio, [FromBody] ClienteDecisionesRequest request)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            var items = await diagnosticoService.DecidirClienteAsync(idOrdenServicio, idUsuario, request.Decisiones);
            return Ok(items);
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}

