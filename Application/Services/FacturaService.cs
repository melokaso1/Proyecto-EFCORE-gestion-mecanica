using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services;

public class FacturaService(IUnitOfWork uow, IMapper mapper) : IFacturaService
{
    public async Task<FacturaDto> GenerarFacturaAsync(int idOrdenServicio, decimal manoObra)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrdenServicio)
            ?? throw new NotFoundException($"Orden de servicio {idOrdenServicio} no encontrada.");

        var estadoOrden = await uow.EstadosOrden.GetByIdAsync(orden.IdEstadoOrden);
        if (estadoOrden?.Nombre != EstadosOrden.Completada)
            throw new BusinessRuleException("Solo se puede facturar una orden en estado Completada.");

        var facturaExistente = (await uow.Facturas.FindAsync(f => f.IdOrdenServicio == idOrdenServicio)).FirstOrDefault();
        if (facturaExistente is not null)
            throw new ConflictException("La orden de servicio ya tiene una factura asociada.");

        var detallesOrden = await uow.DetallesOrden.FindAsync(d => d.IdOrdenServicio == idOrdenServicio);
        var totalRepuestos = detallesOrden.Sum(d => d.Cantidad * d.PrecioUnitarioAplicado);

        var factura = new Factura
        {
            IdOrdenServicio = idOrdenServicio,
            FechaFactura = DateTime.UtcNow,
            ManoObra = manoObra,
            Total = manoObra + totalRepuestos
        };

        await uow.Facturas.AddAsync(factura);
        await uow.CommitAsync();

        foreach (var detalle in detallesOrden)
        {
            var repuesto = await uow.Repuestos.GetByIdAsync(detalle.IdRepuesto);
            await uow.DetallesFactura.AddAsync(new DetalleFactura
            {
                IdFactura = factura.IdFactura,
                Concepto = repuesto?.Descripcion ?? $"Repuesto #{detalle.IdRepuesto}",
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitarioAplicado
            });
        }

        await uow.DetallesFactura.AddAsync(new DetalleFactura
        {
            IdFactura = factura.IdFactura,
            Concepto = "Mano de obra",
            Cantidad = 1,
            PrecioUnitario = manoObra
        });

        await uow.CommitAsync();

        factura.Detalles = (await uow.DetallesFactura.FindAsync(d => d.IdFactura == factura.IdFactura)).ToList();
        return mapper.Map<FacturaDto>(factura);
    }

    public async Task<FacturaDto?> ObtenerPorOrdenAsync(int idOrdenServicio)
    {
        var factura = (await uow.Facturas.FindAsync(f => f.IdOrdenServicio == idOrdenServicio)).FirstOrDefault();
        if (factura is null)
            return null;

        factura.Detalles = (await uow.DetallesFactura.FindAsync(d => d.IdFactura == factura.IdFactura)).ToList();
        return mapper.Map<FacturaDto>(factura);
    }

    public async Task<PagedResultDto<FacturaDto>> ListarAsync(
        int page,
        int size,
        int? idCliente,
        DateTime? desde,
        DateTime? hasta)
    {
        HashSet<int>? idsOrdenes = null;
        if (idCliente is not null)
        {
            var historiales = await uow.HistorialPropietarios.FindAsync(h => h.IdCliente == idCliente);
            var idsVehiculos = historiales.Select(h => h.IdVehiculo).ToHashSet();
            var ordenes = await uow.OrdenesServicio.FindAsync(o => idsVehiculos.Contains(o.IdVehiculo));
            idsOrdenes = ordenes.Select(o => o.IdOrdenServicio).ToHashSet();
        }

        var (items, total) = await uow.Facturas.GetPagedAsync(page, size, f =>
            (desde == null || f.FechaFactura >= desde) &&
            (hasta == null || f.FechaFactura <= hasta) &&
            (idsOrdenes == null || idsOrdenes.Contains(f.IdOrdenServicio)));

        var dtos = new List<FacturaDto>();
        foreach (var factura in items)
        {
            factura.Detalles = (await uow.DetallesFactura.FindAsync(d => d.IdFactura == factura.IdFactura)).ToList();
            dtos.Add(mapper.Map<FacturaDto>(factura));
        }

        return new PagedResultDto<FacturaDto>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = page,
            PageSize = size
        };
    }
}
