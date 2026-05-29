using Frontend.Models;

namespace Frontend.Services;

public interface IVehiculosService
{
    Task<PagedResult<VehiculoListDto>> GetVehiculosAsync(int page, int size, string? vinFiltro, string? placaFiltro = null);
    Task<VehiculoListDto> RegistrarEnCatalogoAsync(CreateVehiculoCasoDto dto);
}
