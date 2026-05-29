using Application.Dtos;

namespace Application.Interfaces;

public interface IVehiculoService
{
    Task<VehiculoDto> CrearAsync(CreateVehiculoDto dto);
    Task<VehiculoDto> CrearEnCatalogoAsync(CreateVehiculoCasoDto dto);
    Task<PagedResultDto<VehiculoDto>> ListarAsync(int page, int size, int? idCliente, string? vin, string? placa);
    Task<VehiculoDto?> ObtenerPorVinAsync(string vin);
    Task<VehiculoDto?> ObtenerPorPlacaAsync(string placa);
    Task<VehiculoDto?> ObtenerPorIdAsync(int id);
    Task ActualizarAsync(int id, CreateVehiculoDto dto);
    Task EliminarAsync(int id);
}
