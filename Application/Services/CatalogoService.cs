using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;

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

    public async Task<IReadOnlyList<MarcaVehiculoDto>> ListarMarcasAsync()
    {
        var items = await uow.MarcasVehiculo.GetAllAsync();
        return items
            .OrderBy(m => m.NombreMarca)
            .Select(m => new MarcaVehiculoDto
            {
                IdMarca = m.IdMarca,
                NombreMarca = m.NombreMarca
            })
            .ToList();
    }

    public async Task<IReadOnlyList<TipoVehiculoDto>> ListarTiposVehiculoAsync()
    {
        var items = await uow.TiposVehiculo.GetAllAsync();
        return items
            .OrderBy(t => t.Nombre)
            .Select(t => new TipoVehiculoDto
            {
                IdTipoVehiculo = t.IdTipoVehiculo,
                Nombre = t.Nombre
            })
            .ToList();
    }

    public async Task<MarcaVehiculoDto> CrearMarcaAsync(CreateMarcaVehiculoDto dto)
    {
        var nombre = dto.NombreMarca.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            throw new BusinessRuleException("El nombre de la marca es obligatorio.");

        var duplicada = (await uow.MarcasVehiculo.FindAsync(m =>
            m.NombreMarca.ToLower() == nombre.ToLower())).FirstOrDefault();
        if (duplicada is not null)
            throw new ConflictException($"Ya existe la marca «{nombre}».");

        var marca = new MarcaVehiculo { NombreMarca = nombre };
        await uow.MarcasVehiculo.AddAsync(marca);
        await uow.CommitAsync();

        return new MarcaVehiculoDto { IdMarca = marca.IdMarca, NombreMarca = marca.NombreMarca };
    }

    public async Task<MarcaVehiculoDto> ActualizarMarcaAsync(int id, CreateMarcaVehiculoDto dto)
    {
        var marca = await uow.MarcasVehiculo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Marca {id} no encontrada.");

        var nombre = dto.NombreMarca.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            throw new BusinessRuleException("El nombre de la marca es obligatorio.");

        var duplicada = (await uow.MarcasVehiculo.FindAsync(m =>
            m.IdMarca != id && m.NombreMarca.ToLower() == nombre.ToLower())).FirstOrDefault();
        if (duplicada is not null)
            throw new ConflictException($"Ya existe la marca «{nombre}».");

        marca.NombreMarca = nombre;
        uow.MarcasVehiculo.Update(marca);
        await uow.CommitAsync();

        return new MarcaVehiculoDto { IdMarca = marca.IdMarca, NombreMarca = marca.NombreMarca };
    }

    public async Task EliminarMarcaAsync(int id)
    {
        var marca = await uow.MarcasVehiculo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Marca {id} no encontrada.");

        var tieneModelos = (await uow.ModelosVehiculo.FindAsync(m => m.IdMarca == id)).Any();
        if (tieneModelos)
            throw new BusinessRuleException("No se puede eliminar una marca con modelos asociados.");

        uow.MarcasVehiculo.Remove(marca);
        await uow.CommitAsync();
    }

    public async Task<ModeloVehiculoCatalogoDto> CrearModeloAsync(CreateModeloVehiculoDto dto)
    {
        var nombre = dto.NombreModelo.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            throw new BusinessRuleException("El nombre del modelo es obligatorio.");

        _ = await uow.MarcasVehiculo.GetByIdAsync(dto.IdMarca)
            ?? throw new NotFoundException($"Marca {dto.IdMarca} no encontrada.");
        _ = await uow.TiposVehiculo.GetByIdAsync(dto.IdTipoVehiculo)
            ?? throw new NotFoundException($"Tipo de vehículo {dto.IdTipoVehiculo} no encontrado.");

        var duplicado = (await uow.ModelosVehiculo.FindAsync(m =>
            m.IdMarca == dto.IdMarca && m.NombreModelo.ToLower() == nombre.ToLower())).FirstOrDefault();
        if (duplicado is not null)
            throw new ConflictException($"Ya existe el modelo «{nombre}» para esa marca.");

        var modelo = new ModeloVehiculo
        {
            IdMarca = dto.IdMarca,
            IdTipoVehiculo = dto.IdTipoVehiculo,
            NombreModelo = nombre
        };
        await uow.ModelosVehiculo.AddAsync(modelo);
        await uow.CommitAsync();

        return await MapModeloAsync(modelo.IdModelo);
    }

    public async Task<ModeloVehiculoCatalogoDto> ActualizarModeloAsync(int id, CreateModeloVehiculoDto dto)
    {
        var modelo = await uow.ModelosVehiculo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Modelo {id} no encontrado.");

        var nombre = dto.NombreModelo.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            throw new BusinessRuleException("El nombre del modelo es obligatorio.");

        _ = await uow.MarcasVehiculo.GetByIdAsync(dto.IdMarca)
            ?? throw new NotFoundException($"Marca {dto.IdMarca} no encontrada.");
        _ = await uow.TiposVehiculo.GetByIdAsync(dto.IdTipoVehiculo)
            ?? throw new NotFoundException($"Tipo de vehículo {dto.IdTipoVehiculo} no encontrado.");

        var duplicado = (await uow.ModelosVehiculo.FindAsync(m =>
            m.IdModelo != id && m.IdMarca == dto.IdMarca && m.NombreModelo.ToLower() == nombre.ToLower()))
            .FirstOrDefault();
        if (duplicado is not null)
            throw new ConflictException($"Ya existe el modelo «{nombre}» para esa marca.");

        modelo.IdMarca = dto.IdMarca;
        modelo.IdTipoVehiculo = dto.IdTipoVehiculo;
        modelo.NombreModelo = nombre;
        uow.ModelosVehiculo.Update(modelo);
        await uow.CommitAsync();

        return await MapModeloAsync(id);
    }

    public async Task EliminarModeloAsync(int id)
    {
        var modelo = await uow.ModelosVehiculo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Modelo {id} no encontrado.");

        var enUso = (await uow.Vehiculos.FindAsync(v => v.IdModelo == id)).Any();
        if (enUso)
            throw new BusinessRuleException("No se puede eliminar un modelo con vehículos registrados.");

        uow.ModelosVehiculo.Remove(modelo);
        await uow.CommitAsync();
    }

    private async Task<ModeloVehiculoCatalogoDto> MapModeloAsync(int idModelo)
    {
        var items = await uow.Vehiculos.ListarModelosCatalogoAsync();
        var m = items.FirstOrDefault(x => x.IdModelo == idModelo)
            ?? throw new NotFoundException($"Modelo {idModelo} no encontrado.");

        return new ModeloVehiculoCatalogoDto
        {
            IdModelo = m.IdModelo,
            Marca = m.Marca?.NombreMarca ?? string.Empty,
            Modelo = m.NombreModelo,
            TipoVehiculo = m.TipoVehiculo?.Nombre ?? string.Empty
        };
    }
}
