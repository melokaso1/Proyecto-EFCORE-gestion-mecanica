using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/especializaciones-mecanico")]
[Authorize]
public class EspecializacionesMecanicoController(IEspecializacionMecanicoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EspecializacionMecanicoDto>>> Listar() =>
        Ok(await service.ListarActivasAsync());
}
