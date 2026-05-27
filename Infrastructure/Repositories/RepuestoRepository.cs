using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RepuestoRepository(AutoTallerDbContext context)
    : GenericRepository<Repuesto>(context), IRepuestoRepository
{
    public override async Task<Repuesto?> GetByIdAsync(int id) =>
        await Context.Repuestos
            .Include(r => r.CategoriaRepuesto)
            .FirstOrDefaultAsync(r => r.IdRepuesto == id);

    public async Task<(IEnumerable<Repuesto> items, int total)> ListarConFiltrosAsync(
        int pageNumber,
        int pageSize,
        string? descripcion,
        int? idCategoria,
        bool? soloConStockMinimo)
    {
        var query = Context.Repuestos
            .Include(r => r.CategoriaRepuesto)
            .AsQueryable();

        if (descripcion is not null)
            query = query.Where(r => r.Descripcion.Contains(descripcion));

        if (idCategoria is not null)
            query = query.Where(r => r.IdCategoriaRepuesto == idCategoria);

        if (soloConStockMinimo == true)
            query = query.Where(r => r.Stock <= r.StockMinimo);

        var total = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
