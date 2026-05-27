using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClienteRepository(AutoTallerDbContext context)
    : GenericRepository<Cliente>(context), IClienteRepository
{
    public override async Task<Cliente?> GetByIdAsync(int id) =>
        await Context.Clientes
            .Include(c => c.Persona)
            .FirstOrDefaultAsync(c => c.IdCliente == id);

    public override async Task<IEnumerable<Cliente>> GetAllAsync() =>
        await Context.Clientes
            .Include(c => c.Persona)
            .ToListAsync();

    public async Task<bool> ExisteConOrdenesActivasAsync(int idCliente)
    {
        var idsVehiculos = await Context.HistorialPropietariosVehiculo
            .Where(h => h.IdCliente == idCliente)
            .Select(h => h.IdVehiculo)
            .ToListAsync();

        return await Context.OrdenesServicio
            .Include(o => o.EstadoOrden)
            .AnyAsync(o => idsVehiculos.Contains(o.IdVehiculo) &&
                           o.EstadoOrden != null &&
                           o.EstadoOrden.Nombre != EstadosOrden.Completada &&
                           o.EstadoOrden.Nombre != EstadosOrden.Cancelada);
    }
}
