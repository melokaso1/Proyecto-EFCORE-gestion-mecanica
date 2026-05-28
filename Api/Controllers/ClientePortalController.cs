using System.Security.Claims;
using Api.Helpers;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Portal autenticado del cliente.</summary>
[ApiController]
[Route("api/portalcliente")]
[Authorize(Roles = "Cliente")]
public class ClientePortalController(IClientePortalService portalService) : ControllerBase
{
    /// <summary>Lista las órdenes del cliente autenticado.</summary>
    [HttpGet("ordenes")]
    public async Task<ActionResult<IEnumerable<ClientePortalOrdenDto>>> MisOrdenes(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? estado = null)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        var result = await portalService.ListarMisOrdenesAsync(idUsuario, pageNumber, pageSize, estado);
        PaginationHelper.AddPaginationHeader(Response, result.TotalCount, pageNumber, pageSize);
        return Ok(result.Items);
    }

    /// <summary>Aprueba o rechaza el costo propuesto de una orden.</summary>
    [HttpPatch("ordenes/{idOrdenServicio:int}/costo/decision")]
    public async Task<IActionResult> DecidirCosto(int idOrdenServicio, [FromBody] ClienteDecisionCostoDto dto)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            await portalService.DecidirCostoAsync(idUsuario, idOrdenServicio, dto);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Lista las reparaciones de una orden propia del cliente.</summary>
    [HttpGet("ordenes/{idOrdenServicio:int}/reparaciones")]
    public async Task<ActionResult<IEnumerable<ReparacionItemDto>>> ListarReparaciones(int idOrdenServicio)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            var items = await portalService.ListarReparacionesOrdenAsync(idUsuario, idOrdenServicio);
            return Ok(items);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

