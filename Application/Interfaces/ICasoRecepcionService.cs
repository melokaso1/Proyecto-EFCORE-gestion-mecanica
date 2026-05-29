using Application.Dtos;

namespace Application.Interfaces;

public interface ICasoRecepcionService
{
    Task<CasoRecepcionDto> IniciarCasoAsync(int idRecepcionista);
    Task<CasoRecepcionDto?> ObtenerCasoAsync(int idOrden);
    Task<PagedResultDto<CasoRecepcionDto>> ListarCasosEnRegistroAsync(int page, int size);
    Task<PagedResultDto<CasoRecepcionDto>> ListarCasosAsync(int page, int size, string? estado);
    Task<ClienteDto?> BuscarClientePorDocumentoAsync(string numeroDocumento);
    Task<VehiculoDto?> BuscarVehiculoPorPlacaAsync(string placa);
    Task<VehiculoDto?> BuscarVehiculoPorVinAsync(string vin);
    Task<CasoRecepcionDto> AsignarClienteAsync(int idOrden, AsignarClienteCasoDto dto);
    Task<CasoRecepcionDto> AsignarVehiculoAsync(int idOrden, AsignarVehiculoCasoDto dto);
    Task<CasoRecepcionDto> ConfirmarCasoAsync(int idOrden, ConfirmarCasoDto dto);
    Task CancelarCasoAsync(int idOrden);
}

public interface ICatalogoService
{
    Task<IReadOnlyList<TipoServicioDto>> ListarTiposServicioAsync();
    Task<IReadOnlyList<ModeloVehiculoCatalogoDto>> ListarModelosVehiculoAsync();
    Task<IReadOnlyList<TipoDocumentoDto>> ListarTiposDocumentoAsync();
    Task<IReadOnlyList<MarcaVehiculoDto>> ListarMarcasAsync();
    Task<IReadOnlyList<TipoVehiculoDto>> ListarTiposVehiculoAsync();
    Task<MarcaVehiculoDto> CrearMarcaAsync(CreateMarcaVehiculoDto dto);
    Task<MarcaVehiculoDto> ActualizarMarcaAsync(int id, CreateMarcaVehiculoDto dto);
    Task EliminarMarcaAsync(int id);
    Task<ModeloVehiculoCatalogoDto> CrearModeloAsync(CreateModeloVehiculoDto dto);
    Task<ModeloVehiculoCatalogoDto> ActualizarModeloAsync(int id, CreateModeloVehiculoDto dto);
    Task EliminarModeloAsync(int id);
}
