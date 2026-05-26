using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services;

public class RepuestoService(IUnitOfWork uow, IMapper mapper) : IRepuestoService
{
    public async Task<RepuestoDto> CrearAsync(CreateRepuestoDto dto)
    {
        var codigoExistente = (await uow.Repuestos.FindAsync(r => r.Codigo == dto.Codigo)).FirstOrDefault();
        if (codigoExistente is not null)
            throw new ConflictException($"Ya existe un repuesto con código {dto.Codigo}.");

        var repuesto = mapper.Map<Repuesto>(dto);
        await uow.Repuestos.AddAsync(repuesto);
        await uow.CommitAsync();

        repuesto.CategoriaRepuesto = await uow.CategoriasRepuesto.GetByIdAsync(repuesto.IdCategoriaRepuesto);
        return mapper.Map<RepuestoDto>(repuesto);
    }

    public async Task ActualizarAsync(int id, CreateRepuestoDto dto)
    {
        var repuesto = await uow.Repuestos.GetByIdAsync(id)
            ?? throw new NotFoundException($"Repuesto {id} no encontrado.");

        var otroCodigo = (await uow.Repuestos.FindAsync(r => r.Codigo == dto.Codigo && r.IdRepuesto != id)).FirstOrDefault();
        if (otroCodigo is not null)
            throw new ConflictException($"Ya existe un repuesto con código {dto.Codigo}.");

        repuesto.IdCategoriaRepuesto = dto.IdCategoriaRepuesto;
        repuesto.Codigo = dto.Codigo;
        repuesto.Descripcion = dto.Descripcion;
        repuesto.Stock = dto.Stock;
        repuesto.StockMinimo = dto.StockMinimo;
        repuesto.PrecioUnitario = dto.PrecioUnitario;
        uow.Repuestos.Update(repuesto);
        await uow.CommitAsync();
    }

    public async Task DesactivarAsync(int id)
    {
        var repuesto = await uow.Repuestos.GetByIdAsync(id)
            ?? throw new NotFoundException($"Repuesto {id} no encontrado.");

        repuesto.Activo = false;
        uow.Repuestos.Update(repuesto);
        await uow.CommitAsync();
    }

    public async Task<PagedResultDto<RepuestoDto>> ListarAsync(
        int page,
        int size,
        string? descripcion,
        int? idCategoria,
        bool? soloConStockMinimo)
    {
        var (items, total) = await uow.Repuestos.GetPagedAsync(page, size, r =>
            (descripcion == null || r.Descripcion.Contains(descripcion)) &&
            (idCategoria == null || r.IdCategoriaRepuesto == idCategoria) &&
            (soloConStockMinimo != true || r.Stock <= r.StockMinimo));

        return new PagedResultDto<RepuestoDto>
        {
            Items = mapper.Map<List<RepuestoDto>>(items),
            TotalCount = total,
            PageNumber = page,
            PageSize = size
        };
    }

    public async Task AjustarStockAsync(int id, int cantidad)
    {
        var repuesto = await uow.Repuestos.GetByIdAsync(id)
            ?? throw new NotFoundException($"Repuesto {id} no encontrado.");

        var nuevoStock = repuesto.Stock + cantidad;
        if (nuevoStock < 0)
            throw new BusinessRuleException("El stock no puede ser negativo.");

        repuesto.Stock = nuevoStock;
        uow.Repuestos.Update(repuesto);
        await uow.CommitAsync();
    }
}
