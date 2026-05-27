using Application.Dtos;

namespace Application.Interfaces;

public interface IFacturaService
{
    Task<FacturaDto> GenerarFacturaAsync(int idOrdenServicio, decimal manoObra);
    Task<FacturaDto?> ObtenerPorIdAsync(int id);
    Task<FacturaDto?> ObtenerPorOrdenAsync(int idOrdenServicio);
    Task<PagedResultDto<FacturaDto>> ListarAsync(
        int page,
        int size,
        int? idCliente,
        DateTime? desde,
        DateTime? hasta);
}
