using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VehiculoRepository(AutoTallerDbContext context)
    : GenericRepository<Vehiculo>(context), IVehiculoRepository
{
    public override async Task<Vehiculo?> GetByIdAsync(int id) =>
        await Context.Vehiculos
            .Include(v => v.Modelo!)
            .ThenInclude(m => m.Marca)
            .FirstOrDefaultAsync(v => v.IdVehiculo == id);

    public async Task<Vehiculo?> ObtenerPorVinAsync(string vin) =>
        await Context.Vehiculos
            .Include(v => v.Modelo!)
            .ThenInclude(m => m.Marca)
            .FirstOrDefaultAsync(v => v.VIN == vin);

    public async Task<bool> ExisteConOrdenesActivasAsync(int idVehiculo) =>
        await Context.OrdenesServicio
            .Include(o => o.EstadoOrden)
            .AnyAsync(o => o.IdVehiculo == idVehiculo &&
                           o.EstadoOrden != null &&
                           o.EstadoOrden.Nombre != EstadosOrden.Completada &&
                           o.EstadoOrden.Nombre != EstadosOrden.Cancelada);
}
