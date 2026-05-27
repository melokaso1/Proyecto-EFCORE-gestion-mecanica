using Api.Helpers;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Operaciones CRUD de clientes.</summary>
[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClientesController(IClienteService clienteService) : ControllerBase
{
    /// <summary>Lista clientes paginados.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filtro = null)
    {
        var result = await clienteService.ListarClientesAsync(pageNumber, pageSize, filtro);
        PaginationHelper.AddPaginationHeader(Response, result.TotalCount, pageNumber, pageSize);
        return Ok(result.Items);
    }

    /// <summary>Obtiene un cliente por id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDto>> Obtener(int id)
    {
        var cliente = await clienteService.ObtenerPorIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    /// <summary>Registra un nuevo cliente con sus datos de contacto.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<ClienteDto>> Crear([FromBody] CreateClienteDto dto)
    {
        var cliente = await clienteService.RegistrarClienteConVehiculoAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = cliente.IdCliente }, cliente);
    }

    /// <summary>Actualiza los datos de un cliente.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CreateClienteDto dto)
    {
        try
        {
            await clienteService.ActualizarAsync(id, dto);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Elimina un cliente si no tiene órdenes activas.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await clienteService.EliminarAsync(id);
        return NoContent();
    }
}
