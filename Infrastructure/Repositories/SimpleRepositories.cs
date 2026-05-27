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
}

public class AuditoriaRepository(AutoTallerDbContext context)
    : GenericRepository<Auditoria>(context), IAuditoriaRepository;

public class DetalleOrdenRepository(AutoTallerDbContext context)
    : GenericRepository<DetalleOrdenRepuesto>(context), IDetalleOrdenRepository;
