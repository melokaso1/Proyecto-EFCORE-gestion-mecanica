using System.Security.Claims;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/caja")]
[Authorize(Roles = "Admin,Recepcionista")]
public class CajaController(ICajaService caja) : ControllerBase
{
    [HttpPost("pagos")]
    public async Task<ActionResult<PagoYFacturaDto>> RegistrarPago([FromBody] RegistrarPagoDto dto)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(idClaim, out var idUsuario))
            return Unauthorized();

        try
        {
            var result = await caja.RegistrarPagoAsync(idUsuario, dto);
            return Ok(result);
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

