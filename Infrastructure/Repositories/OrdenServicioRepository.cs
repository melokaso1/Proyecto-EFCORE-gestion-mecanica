using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrdenServicioRepository(AutoTallerDbContext context)
    : GenericRepository<OrdenServicio>(context), IOrdenServicioRepository
{
    public override async Task<OrdenServicio?> GetByIdAsync(int id) =>
        await Context.OrdenesServicio
            .Include(o => o.Vehiculo!)
            .ThenInclude(v => v.Modelo!)
            .ThenInclude(m => m.Marca)
            .Include(o => o.Vehiculo!)
            .ThenInclude(v => v.Modelo!)
            .ThenInclude(m => m.TipoVehiculo)
            .Include(o => o.Cliente!)
            .ThenInclude(c => c.Persona)
            .Include(o => o.Vehiculo)
            .Include(o => o.TipoServicio)
            .Include(o => o.Mecanico!)
            .ThenInclude(m => m.Persona)
            .Include(o => o.EstadoOrden)
            .FirstOrDefaultAsync(o => o.IdOrdenServicio == id);

    public async Task<(IEnumerable<OrdenServicio> items, int total)> GetPagedConFiltrosAsync(
        int pageNumber,
        int pageSize,
        string? estado,
        int? idCliente,
        int? idMecanico,
        DateTime? desde,
        DateTime? hasta)
    {
        var query = Context.OrdenesServicio
            .Include(o => o.Vehiculo!)
            .ThenInclude(v => v.Modelo!)
            .ThenInclude(m => m.Marca)
            .Include(o => o.Vehiculo!)
            .ThenInclude(v => v.Modelo!)
            .ThenInclude(m => m.TipoVehiculo)
            .Include(o => o.Cliente!)
            .ThenInclude(c => c.Persona)
            .Include(o => o.Vehiculo)
            .Include(o => o.TipoServicio)
            .Include(o => o.Mecanico!)
            .ThenInclude(m => m.Persona)
            .Include(o => o.EstadoOrden)
            .AsQueryable();

        if (idMecanico is not null)
            query = query.Where(o => o.IdMecanico == idMecanico);

        if (desde is not null)
            query = query.Where(o => o.FechaIngreso >= desde);

        if (hasta is not null)
            query = query.Where(o => o.FechaIngreso <= hasta);

        if (estado is not null)
            query = query.Where(o => o.EstadoOrden != null && o.EstadoOrden.Nombre == estado);

        if (idCliente is not null)
            query = query.Where(o => o.IdCliente == idCliente);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(o => o.FechaIngreso)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
