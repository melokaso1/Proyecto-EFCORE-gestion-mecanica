using System.Security.Claims;
using Api.Helpers;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/casos")]
[Authorize(Roles = "Admin,Recepcionista")]
public class CasosController(ICasoRecepcionService casoService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CasoRecepcionDto>> Iniciar()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idRecepcionista))
            return Unauthorized();

        var caso = await casoService.IniciarCasoAsync(idRecepcionista);
        return CreatedAtAction(nameof(Obtener), new { id = caso.IdOrdenServicio }, caso);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CasoRecepcionDto>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? estado = null)
    {
        string? estadoFiltro = estado?.Trim() switch
        {
            null or "" or "En registro" => Domain.Constants.EstadosOrden.EnRegistro,
            "todos" => null,
            _ => estado!.Trim()
        };

        var result = await casoService.ListarCasosAsync(pageNumber, pageSize, estadoFiltro);
        PaginationHelper.AddPaginationHeader(Response, result.TotalCount, pageNumber, pageSize);
        return Ok(result.Items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CasoRecepcionDto>> Obtener(int id)
    {
        var caso = await casoService.ObtenerCasoAsync(id);
        return caso is null ? NotFound() : Ok(caso);
    }

    [HttpGet("buscar/cliente")]
    public async Task<ActionResult<ClienteDto>> BuscarCliente([FromQuery] string documento)
    {
        var cliente = await casoService.BuscarClientePorDocumentoAsync(documento);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpGet("buscar/vehiculo")]
    public async Task<ActionResult<VehiculoDto>> BuscarVehiculo(
        [FromQuery] string? placa,
        [FromQuery] string? vin)
    {
        VehiculoDto? vehiculo = null;
        if (!string.IsNullOrWhiteSpace(placa))
            vehiculo = await casoService.BuscarVehiculoPorPlacaAsync(placa);
        else if (!string.IsNullOrWhiteSpace(vin))
            vehiculo = await casoService.BuscarVehiculoPorVinAsync(vin);

        return vehiculo is null ? NotFound() : Ok(vehiculo);
    }

    [HttpPatch("{id:int}/cliente")]
    public async Task<ActionResult<CasoRecepcionDto>> AsignarCliente(int id, [FromBody] AsignarClienteCasoDto dto)
    {
        try
        {
            return Ok(await casoService.AsignarClienteAsync(id, dto));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/vehiculo")]
    public async Task<ActionResult<CasoRecepcionDto>> AsignarVehiculo(int id, [FromBody] AsignarVehiculoCasoDto dto)
    {
        try
        {
            return Ok(await casoService.AsignarVehiculoAsync(id, dto));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/confirmar")]
    public async Task<ActionResult<CasoRecepcionDto>> Confirmar(int id, [FromBody] ConfirmarCasoDto dto)
    {
        try
        {
            return Ok(await casoService.ConfirmarCasoAsync(id, dto));
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

    [HttpPatch("{id:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        try
        {
            await casoService.CancelarCasoAsync(id);
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
}
