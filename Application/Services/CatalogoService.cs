using Application.Dtos;
using Application.Interfaces;

namespace Application.Services;

public class CatalogoService(IUnitOfWork uow) : ICatalogoService
{
    public async Task<IReadOnlyList<TipoServicioDto>> ListarTiposServicioAsync()
    {
        var items = await uow.TiposServicio.GetAllAsync();
        return items
            .OrderBy(t => t.Nombre)
            .Select(t => new TipoServicioDto
            {
                IdTipoServicio = t.IdTipoServicio,
                Nombre = t.Nombre
            })
            .ToList();
    }

    public async Task<IReadOnlyList<ModeloVehiculoCatalogoDto>> ListarModelosVehiculoAsync()
    {
        var items = await uow.Vehiculos.ListarModelosCatalogoAsync();
        return items
            .Select(m => new ModeloVehiculoCatalogoDto
            {
                IdModelo = m.IdModelo,
                Marca = m.Marca?.NombreMarca ?? string.Empty,
                Modelo = m.NombreModelo,
                TipoVehiculo = m.TipoVehiculo?.Nombre ?? string.Empty
            })
            .ToList();
    }

    public async Task<IReadOnlyList<TipoDocumentoDto>> ListarTiposDocumentoAsync()
    {
        var items = await uow.TiposDocumento.GetAllAsync();
        return items
            .OrderBy(t => t.IdTipoDocumento)
            .Select(t => new TipoDocumentoDto
            {
                IdTipoDocumento = t.IdTipoDocumento,
                Codigo = t.Codigo,
                Nombre = t.Nombre
            })
            .ToList();
    }
}
