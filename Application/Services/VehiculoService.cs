using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services;

public class VehiculoService(IUnitOfWork uow, IMapper mapper) : IVehiculoService
{
    public async Task<VehiculoDto> CrearAsync(CreateVehiculoDto dto)
    {
        var vinExistente = (await uow.Vehiculos.FindAsync(v => v.VIN == dto.VIN)).FirstOrDefault();
        if (vinExistente is not null)
            throw new ConflictException($"Ya existe un vehículo con VIN {dto.VIN}.");

        var vehiculo = mapper.Map<Vehiculo>(dto);
        await uow.Vehiculos.AddAsync(vehiculo);
        await uow.CommitAsync();

        await uow.HistorialPropietarios.AddAsync(new HistorialPropietarioVehiculo
        {
            IdVehiculo = vehiculo.IdVehiculo,
            IdCliente = dto.IdCliente,
            FechaInicio = DateTime.UtcNow
        });
        await uow.CommitAsync();

        return mapper.Map<VehiculoDto>(vehiculo);
    }

    public async Task<PagedResultDto<VehiculoDto>> ListarAsync(int page, int size, int? idCliente, string? vin)
    {
        HashSet<int>? idsVehiculosCliente = null;
        if (idCliente is not null)
        {
            var historiales = await uow.HistorialPropietarios.FindAsync(h =>
                h.IdCliente == idCliente && h.FechaFin == null);
            idsVehiculosCliente = historiales.Select(h => h.IdVehiculo).ToHashSet();
        }

        var (items, total) = await uow.Vehiculos.GetPagedAsync(page, size, v =>
            (vin == null || v.VIN.Contains(vin)) &&
            (idsVehiculosCliente == null || idsVehiculosCliente.Contains(v.IdVehiculo)));

        return new PagedResultDto<VehiculoDto>
        {
            Items = mapper.Map<List<VehiculoDto>>(items),
            TotalCount = total,
            PageNumber = page,
            PageSize = size
        };
    }

    public async Task<VehiculoDto?> ObtenerPorVinAsync(string vin)
    {
        var vehiculo = (await uow.Vehiculos.FindAsync(v => v.VIN == vin)).FirstOrDefault();
        return vehiculo is null ? null : mapper.Map<VehiculoDto>(vehiculo);
    }

    public async Task<VehiculoDto?> ObtenerPorIdAsync(int id)
    {
        var vehiculo = await uow.Vehiculos.GetByIdAsync(id);
        return vehiculo is null ? null : mapper.Map<VehiculoDto>(vehiculo);
    }

    public async Task ActualizarAsync(int id, CreateVehiculoDto dto)
    {
        var vehiculo = await uow.Vehiculos.GetByIdAsync(id)
            ?? throw new NotFoundException($"Vehículo {id} no encontrado.");

        var otroVin = (await uow.Vehiculos.FindAsync(v => v.VIN == dto.VIN && v.IdVehiculo != id)).FirstOrDefault();
        if (otroVin is not null)
            throw new ConflictException($"Ya existe un vehículo con VIN {dto.VIN}.");

        vehiculo.IdModelo = dto.IdModelo;
        vehiculo.VIN = dto.VIN;
        vehiculo.Anio = dto.Anio;
        vehiculo.Kilometraje = dto.Kilometraje;
        uow.Vehiculos.Update(vehiculo);
        await uow.CommitAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var vehiculo = await uow.Vehiculos.GetByIdAsync(id)
            ?? throw new NotFoundException($"Vehículo {id} no encontrado.");

        var ordenes = await uow.OrdenesServicio.FindAsync(o => o.IdVehiculo == id);
        if (ordenes.Any(o => o.EstaActiva()))
            throw new BusinessRuleException("No se puede eliminar un vehículo con órdenes de servicio activas.");

        uow.Vehiculos.Remove(vehiculo);
        await uow.CommitAsync();
    }
}
