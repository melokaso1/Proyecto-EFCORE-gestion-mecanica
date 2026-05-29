using Api.Helpers;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Operaciones CRUD de vehículos.</summary>
[ApiController]
[Route("api/vehiculos")]
[Authorize]
public class VehiculosController(IVehiculoService vehiculoService) : ControllerBase
{
    /// <summary>Lista vehículos paginados.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehiculoDto>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? idCliente = null,
        [FromQuery] string? vin = null,
        [FromQuery] string? placa = null)
    {
        var result = await vehiculoService.ListarAsync(pageNumber, pageSize, idCliente, vin, placa);
        PaginationHelper.AddPaginationHeader(Response, result.TotalCount, pageNumber, pageSize);

        // #region agent log
        Api.DebugSessionLogger.Log(
            location: "Api/Controllers/VehiculosController.cs:Listar",
            message: "Vehiculos listed",
            data: new
            {
                pageNumber,
                pageSize,
                idCliente,
                vinLen = vin?.Length,
                total = result.TotalCount,
                returned = result.Items.Count,
                tipoVehiculoSample = result.Items.FirstOrDefault()?.TipoVehiculo
            },
            hypothesisId: "H3-tipo-vehiculo-missing",
            runId: "pre-fix");
        // #endregion agent log

        return Ok(result.Items);
    }

    /// <summary>Obtiene un vehículo por id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehiculoDto>> Obtener(int id)
    {
        var vehiculo = await vehiculoService.ObtenerPorIdAsync(id);
        return vehiculo is null ? NotFound() : Ok(vehiculo);
    }

    /// <summary>Obtiene un vehículo por VIN.</summary>
    [HttpGet("vin/{vin}")]
    public async Task<ActionResult<VehiculoDto>> ObtenerPorVin(string vin)
    {
        var vehiculo = await vehiculoService.ObtenerPorVinAsync(vin);
        return vehiculo is null ? NotFound() : Ok(vehiculo);
    }

    /// <summary>Registra un vehículo en catálogo sin asignar propietario.</summary>
    [HttpPost("catalogo")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<VehiculoDto>> CrearEnCatalogo([FromBody] CreateVehiculoCasoDto dto)
    {
        try
        {
            var vehiculo = await vehiculoService.CrearEnCatalogoAsync(dto);
            return CreatedAtAction(nameof(Obtener), new { id = vehiculo.IdVehiculo }, vehiculo);
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

    /// <summary>Registra un nuevo vehículo.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<VehiculoDto>> Crear([FromBody] CreateVehiculoDto dto)
    {
        var vehiculo = await vehiculoService.CrearAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = vehiculo.IdVehiculo }, vehiculo);
    }

    /// <summary>Actualiza un vehículo.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CreateVehiculoDto dto)
    {
        try
        {
            await vehiculoService.ActualizarAsync(id, dto);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Elimina un vehículo si no tiene órdenes activas.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await vehiculoService.EliminarAsync(id);
        return NoContent();
    }
}
