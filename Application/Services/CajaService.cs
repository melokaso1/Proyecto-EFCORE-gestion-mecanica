using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services;

public class CajaService(IUnitOfWork uow, IFacturaService facturaService, IAuditoriaService auditoria, IMapper mapper)
    : ICajaService
{
    public async Task<PagoYFacturaDto> RegistrarPagoAsync(int idUsuarioCaja, RegistrarPagoDto dto)
    {
        if (dto.IdOrdenServicio <= 0)
            throw new BusinessRuleException("IdOrdenServicio inválido.");
        if (dto.Monto <= 0)
            throw new BusinessRuleException("Monto inválido.");
        if (string.IsNullOrWhiteSpace(dto.Metodo))
            throw new BusinessRuleException("Método de pago requerido.");

        var metodo = dto.Metodo.Trim().ToLowerInvariant();
        if (metodo is not ("efectivo" or "transferencia" or "tarjeta"))
            throw new BusinessRuleException("Método inválido. Use: efectivo/transferencia/tarjeta.");

        var orden = await uow.OrdenesServicio.GetByIdAsync(dto.IdOrdenServicio)
            ?? throw new NotFoundException($"Orden {dto.IdOrdenServicio} no encontrada.");

        var estado = await uow.EstadosOrden.GetByIdAsync(orden.IdEstadoOrden);
        if (estado?.Nombre is EstadosOrden.Cancelada or EstadosOrden.Cerrado)
            throw new BusinessRuleException("No se puede pagar una orden cancelada/cerrada.");

        var pago = new Pago
        {
            IdOrdenServicio = dto.IdOrdenServicio,
            FechaPago = DateTime.UtcNow,
            Metodo = metodo,
            Monto = dto.Monto,
            Referencia = string.IsNullOrWhiteSpace(dto.Referencia) ? null : dto.Referencia.Trim()
        };

        await uow.Pagos.AddAsync(pago);

        var estadoPagado = (await uow.EstadosOrden.FindAsync(e => e.Nombre == EstadosOrden.Pagado)).FirstOrDefault();
        if (estadoPagado is not null)
        {
            orden.IdEstadoOrden = estadoPagado.IdEstadoOrden;
            uow.OrdenesServicio.Update(orden);
        }

        await uow.CommitAsync();

        FacturaDto? factura = null;
        try
        {
            factura = await facturaService.ObtenerPorOrdenAsync(dto.IdOrdenServicio);
            if (factura is null)
                factura = await facturaService.GenerarFacturaAsync(dto.IdOrdenServicio, dto.ManoObra);
        }
        catch
        {
            // si falla la factura, el pago igual queda registrado; la caja puede reintentar generar factura
        }

        await auditoria.RegistrarAsync(
            idUsuarioCaja,
            "Creación",
            nameof(Pago),
            pago.IdPago,
            $"Pago registrado ({metodo})");

        return new PagoYFacturaDto
        {
            Pago = mapper.Map<PagoDto>(pago),
            Factura = factura
        };
    }

    public async Task<bool> OrdenEstaPagadaAsync(int idOrdenServicio)
    {
        var pagos = await uow.Pagos.FindAsync(p => p.IdOrdenServicio == idOrdenServicio);
        return pagos.Any();
    }

    public async Task<IReadOnlyList<OrdenPendientePagoDto>> ListarOrdenesPendientesPagoAsync()
    {
        var (items, _) = await uow.OrdenesServicio.GetPagedConFiltrosAsync(
            1, 100, EstadosOrden.PendientePago, null, null, null, null);

        var result = new List<OrdenPendientePagoDto>();
        foreach (var orden in items)
        {
            var clienteNombre = orden.Cliente?.Persona is null
                ? string.Empty
                : $"{orden.Cliente.Persona.Nombres} {orden.Cliente.Persona.Apellidos}".Trim();

            result.Add(new OrdenPendientePagoDto
            {
                IdOrdenServicio = orden.IdOrdenServicio,
                ClienteNombre = clienteNombre,
                Placa = orden.Vehiculo?.Placa ?? string.Empty,
                TotalEstimado = await CalcularTotalEstimadoAsync(orden)
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<PagoDto>> ListarPagosRecientesAsync(int limit = 20)
    {
        if (limit <= 0)
            limit = 20;

        var pagos = (await uow.Pagos.FindAsync(_ => true))
            .OrderByDescending(p => p.FechaPago)
            .Take(limit)
            .ToList();

        return mapper.Map<List<PagoDto>>(pagos);
    }

    private async Task<decimal> CalcularTotalEstimadoAsync(OrdenServicio orden)
    {
        if (orden.CostoPropuesto is > 0)
            return orden.CostoPropuesto.Value;

        var detallesOrden = await uow.DetallesOrden.FindAsync(d => d.IdOrdenServicio == orden.IdOrdenServicio);
        var totalRepuestos = detallesOrden.Sum(d => d.Cantidad * d.PrecioUnitarioAplicado);

        var reparaciones = await uow.ReparacionesItem.FindAsync(r =>
            r.IdOrdenServicio == orden.IdOrdenServicio && r.Estado == EstadosReparacionItem.Terminado);
        var totalReparaciones = reparaciones.Sum(r => r.CostoEstimado);

        return totalRepuestos + totalReparaciones;
    }
}

