using Application.Dtos;

namespace Application.Interfaces;

public interface IRepuestoService
{
    Task<RepuestoDto> CrearAsync(CreateRepuestoDto dto);
    Task ActualizarAsync(int id, CreateRepuestoDto dto);
    Task DesactivarAsync(int id);
    Task<PagedResultDto<RepuestoDto>> ListarAsync(
        int page,
        int size,
        string? descripcion,
        int? idCategoria,
        bool? soloConStockMinimo);
    Task<RepuestoDto?> ObtenerPorIdAsync(int id);
    Task AjustarStockAsync(int id, int cantidad);
}
