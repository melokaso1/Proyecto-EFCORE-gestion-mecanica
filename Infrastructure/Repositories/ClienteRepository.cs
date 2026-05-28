using System.Linq.Expressions;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClienteRepository(AutoTallerDbContext context)
    : GenericRepository<Cliente>(context), IClienteRepository
{
    public override async Task<(IEnumerable<Cliente> items, int total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<Cliente, bool>>? filter = null)
    {
        var query = Context.Clientes
            .Include(c => c.Persona)
            .ThenInclude(p => p!.CorreosPersona)
            .ThenInclude(cp => cp.DominioCorreo)
            .Include(c => c.Persona)
            .ThenInclude(p => p!.TelefonosPersona)
            .ThenInclude(tp => tp.CodigoTelefono)
            .AsQueryable();

        if (filter is not null)
            query = query.Where(filter);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.IdCliente)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public override async Task<Cliente?> GetByIdAsync(int id) =>
        await Context.Clientes
            .Include(c => c.Persona)
            .ThenInclude(p => p!.CorreosPersona)
            .ThenInclude(cp => cp.DominioCorreo)
            .Include(c => c.Persona)
            .ThenInclude(p => p!.TelefonosPersona)
            .ThenInclude(tp => tp.CodigoTelefono)
            .FirstOrDefaultAsync(c => c.IdCliente == id);

    public override async Task<IEnumerable<Cliente>> GetAllAsync() =>
        await Context.Clientes
            .Include(c => c.Persona)
            .ThenInclude(p => p!.CorreosPersona)
            .ThenInclude(cp => cp.DominioCorreo)
            .Include(c => c.Persona)
            .ThenInclude(p => p!.TelefonosPersona)
            .ThenInclude(tp => tp.CodigoTelefono)
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
