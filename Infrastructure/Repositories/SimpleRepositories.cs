using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FacturaRepository(AutoTallerDbContext context)
    : GenericRepository<Factura>(context), IFacturaRepository
{
    public override async Task<Factura?> GetByIdAsync(int id) =>
        await Context.Facturas
            .Include(f => f.Detalles)
            .FirstOrDefaultAsync(f => f.IdFactura == id);
}

public class UsuarioRepository(AutoTallerDbContext context)
    : GenericRepository<Usuario>(context), IUsuarioRepository
{
    public override async Task<Usuario?> GetByIdAsync(int id) =>
        await Context.Usuarios
            .Include(u => u.Persona!)
            .ThenInclude(p => p.CorreosPersona)
            .Include(u => u.Roles)
            .Include(u => u.Especializaciones)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

    public override async Task<(IEnumerable<Usuario> items, int total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        System.Linq.Expressions.Expression<Func<Usuario, bool>>? filter = null)
    {
        var query = Context.Usuarios
            .Include(u => u.Persona!)
            .ThenInclude(p => p.CorreosPersona)
            .ThenInclude(c => c.DominioCorreo)
            .Include(u => u.Roles)
            .Include(u => u.Especializaciones)
            .AsQueryable();

        if (filter is not null)
            query = query.Where(filter);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.IdUsuario)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<bool> ExisteConRolAsync(string nombreRol) =>
        await Context.Usuarios
            .Include(u => u.Roles)
            .AnyAsync(u => u.Roles.Any(r => r.NombreRol == nombreRol));

    public async Task<(IEnumerable<Usuario> items, int total)> GetEmpleadosPagedAsync(
        int pageNumber,
        int pageSize)
    {
        var query = Context.Usuarios
            .Include(u => u.Persona!)
            .ThenInclude(p => p.CorreosPersona)
            .ThenInclude(c => c.DominioCorreo)
            .Include(u => u.Roles)
            .Include(u => u.Especializaciones)
            .Where(u => u.Estado && u.Roles.Any(r => r.NombreRol == "Mecánico" || r.NombreRol == "Recepcionista"))
            .OrderBy(u => u.IdUsuario);

        var total = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public Task<bool> TieneEspecializacionAsync(int idUsuario, int idEspecializacion) =>
        Context.Usuarios
            .Include(u => u.Especializaciones)
            .AnyAsync(u => u.IdUsuario == idUsuario &&
                           u.Especializaciones.Any(e => e.IdEspecializacionMecanico == idEspecializacion && e.Activo));

    public Task<bool> TieneEspecializacionPorCodigoAsync(int idUsuario, string codigo) =>
        Context.Usuarios
            .Include(u => u.Especializaciones)
            .AnyAsync(u => u.IdUsuario == idUsuario &&
                           u.Especializaciones.Any(e => e.Codigo == codigo && e.Activo));
}

public class AuditoriaRepository(AutoTallerDbContext context)
    : GenericRepository<Auditoria>(context), IAuditoriaRepository;

public class DetalleOrdenRepository(AutoTallerDbContext context)
    : GenericRepository<DetalleOrdenRepuesto>(context), IDetalleOrdenRepository;

public class ReparacionItemRepository(AutoTallerDbContext context)
    : GenericRepository<ReparacionItem>(context), IReparacionItemRepository
{
    public async Task<IReadOnlyList<ReparacionItem>> ListarPorOrdenConDetalleAsync(int idOrdenServicio) =>
        await Context.ReparacionesItem
            .Include(r => r.Especializacion)
            .Include(r => r.Mecanico!)
            .ThenInclude(m => m.Persona)
            .Where(r => r.IdOrdenServicio == idOrdenServicio)
            .OrderBy(r => r.Orden)
            .ToListAsync();

    public async Task<IReadOnlyList<ReparacionItem>> ListarPendientesJefeConDetalleAsync(int page, int size) =>
        await Context.ReparacionesItem
            .Include(r => r.Especializacion)
            .Include(r => r.Mecanico!)
            .ThenInclude(m => m.Persona)
            .Where(r => r.Estado == EstadosReparacionItem.PendienteAprobacionJefe)
            .OrderBy(r => r.IdOrdenServicio)
            .ThenBy(r => r.Orden)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

    public async Task<ReparacionItem?> GetDetalleAsync(int idReparacionItem) =>
        await Context.ReparacionesItem
            .Include(r => r.Especializacion)
            .Include(r => r.Mecanico!)
            .ThenInclude(m => m.Persona)
            .FirstOrDefaultAsync(r => r.IdReparacionItem == idReparacionItem);
}
