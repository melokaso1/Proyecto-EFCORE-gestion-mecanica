using Application.Dtos;

namespace Application.Interfaces;

public interface IOrdenServicioService
{
    Task<OrdenServicioDto> CrearOrdenAsync(CreateOrdenServicioDto dto);
    Task ActualizarConTrabajoRealizadoAsync(int id, UpdateOrdenServicioDto dto);
    Task CancelarOrdenAsync(int id);
    Task<PagedResultDto<OrdenServicioDto>> ListarAsync(
        int page,
        int size,
        string? estado,
        int? idCliente,
        int? idMecanico,
        DateTime? desde,
        DateTime? hasta);
    Task<OrdenServicioDto?> ObtenerPorIdAsync(int id);
    Task<SeguimientoOrdenDto?> ConsultarSeguimientoAsync(string documento, string? placa, int? codigoOrden);
}
