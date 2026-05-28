using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class VehiculoRepository(AutoTallerDbContext context)
    : GenericRepository<Vehiculo>(context), IVehiculoRepository
{
    public override async Task<(IEnumerable<Vehiculo> items, int total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<Vehiculo, bool>>? filter = null)
    {
        // #region agent log
        Infrastructure.DebugSessionLogger.Log(
            location: "Infrastructure/Repositories/VehiculoRepository.cs:GetPagedAsync:enter",
            message: "GetPagedAsync called",
            data: new { pageNumber, pageSize, hasFilter = filter is not null },
            hypothesisId: "H4-include-or-mapping",
            runId: "pre-fix");
        // #endregion agent log

        var query = Context.Vehiculos
            .Include(v => v.Modelo!)
            .ThenInclude(m => m.Marca)
            .Include(v => v.Modelo!)
            .ThenInclude(m => m.TipoVehiculo)
            .AsQueryable();

        if (filter is not null)
            query = query.Where(filter);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(v => v.IdVehiculo)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // #region agent log
        var sample = items.FirstOrDefault();
        Infrastructure.DebugSessionLogger.Log(
            location: "Infrastructure/Repositories/VehiculoRepository.cs:GetPagedAsync:exit",
            message: "GetPagedAsync returned",
            data: new
            {
                total,
                returned = items.Count,
                sampleHasModelo = sample?.Modelo is not null,
                sampleHasTipo = sample?.Modelo?.TipoVehiculo is not null,
                sampleTipoNombreLen = sample?.Modelo?.TipoVehiculo?.Nombre?.Length
            },
            hypothesisId: "H4-include-or-mapping",
            runId: "pre-fix");
        // #endregion agent log

        return (items, total);
    }

    public override async Task<Vehiculo?> GetByIdAsync(int id) =>
        await Context.Vehiculos
            .Include(v => v.Modelo!)
            .ThenInclude(m => m.Marca)
            .Include(v => v.Modelo!)
            .ThenInclude(m => m.TipoVehiculo)
            .FirstOrDefaultAsync(v => v.IdVehiculo == id);

    public async Task<Vehiculo?> ObtenerPorVinAsync(string vin) =>
        await Context.Vehiculos
            .Include(v => v.Modelo!)
            .ThenInclude(m => m.Marca)
            .Include(v => v.Modelo!)
            .ThenInclude(m => m.TipoVehiculo)
            .FirstOrDefaultAsync(v => v.VIN == vin);

    public async Task<bool> ExisteConOrdenesActivasAsync(int idVehiculo) =>
        await Context.OrdenesServicio
            .Include(o => o.EstadoOrden)
            .AnyAsync(o => o.IdVehiculo == idVehiculo &&
                           o.EstadoOrden != null &&
                           o.EstadoOrden.Nombre != EstadosOrden.Completada &&
                           o.EstadoOrden.Nombre != EstadosOrden.Cancelada);
}
