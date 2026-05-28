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
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

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
            .Where(u => u.Roles.Any(r => r.NombreRol == "Mecánico" || r.NombreRol == "Recepcionista"))
            .OrderBy(u => u.IdUsuario);

        var total = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}

public class AuditoriaRepository(AutoTallerDbContext context)
    : GenericRepository<Auditoria>(context), IAuditoriaRepository;

public class DetalleOrdenRepository(AutoTallerDbContext context)
    : GenericRepository<DetalleOrdenRepuesto>(context), IDetalleOrdenRepository;
