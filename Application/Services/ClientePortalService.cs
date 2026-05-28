using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class ClientePortalService(IUnitOfWork uow, IFacturaService facturaService) : IClientePortalService
{
    public async Task<PagedResultDto<ClientePortalOrdenDto>> ListarMisOrdenesAsync(
        int idUsuario,
        int page,
        int size,
        string? estado)
    {
        var cliente = await ObtenerClientePorUsuarioAsync(idUsuario);

        var (items, total) = await uow.OrdenesServicio.GetPagedConFiltrosAsync(
            page, size, estado, cliente.IdCliente, idMecanico: null, desde: null, hasta: null);

        var dtos = new List<ClientePortalOrdenDto>();
        foreach (var orden in items)
        {
            // seguridad extra: GetPagedConFiltrosAsync filtra por historial, pero validamos que exista propietario actual
            if (!await ClienteEsPropietarioActualVehiculoAsync(cliente.IdCliente, orden.IdVehiculo))
                continue;

            var vehiculo = orden.Vehiculo ?? await uow.Vehiculos.GetByIdAsync(orden.IdVehiculo);
            if (vehiculo?.Modelo is null)
                vehiculo = await uow.Vehiculos.GetByIdAsync(orden.IdVehiculo);

            decimal? totalFactura = null;
            try
            {
                var factura = await facturaService.ObtenerPorOrdenAsync(orden.IdOrdenServicio);
                totalFactura = factura?.Total;
            }
            catch
            {
                // Si no hay factura todavía, no es error para el portal.
            }

            var marcaModelo = vehiculo?.Modelo?.Marca is null
                ? string.Empty
                : $"{vehiculo.Modelo.Marca.NombreMarca} {vehiculo.Modelo.NombreModelo}".Trim();

            dtos.Add(new ClientePortalOrdenDto
            {
                IdOrdenServicio = orden.IdOrdenServicio,
                VIN = vehiculo?.VIN ?? string.Empty,
                MarcaModelo = marcaModelo,
                Anio = vehiculo?.Anio ?? 0,
                TipoServicio = orden.TipoServicio?.Nombre ?? string.Empty,
                Estado = orden.EstadoOrden?.Nombre ?? string.Empty,
                FechaIngreso = orden.FechaIngreso,
                FechaEstimadaEntrega = orden.FechaEstimadaEntrega,
                TrabajoRealizado = orden.TrabajoRealizado,
                CostoPropuesto = orden.CostoPropuesto,
                NotaCostoPropuesto = orden.NotaCostoPropuesto,
                CostoAprobado = orden.CostoAprobado,
                FechaDecisionCosto = orden.FechaDecisionCosto,
                ComentarioCliente = orden.ComentarioCliente,
                TotalFactura = totalFactura
            });
        }

        return new PagedResultDto<ClientePortalOrdenDto>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = page,
            PageSize = size
        };
    }

    public async Task DecidirCostoAsync(int idUsuario, int idOrdenServicio, ClienteDecisionCostoDto dto)
    {
        var cliente = await ObtenerClientePorUsuarioAsync(idUsuario);

        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrdenServicio)
            ?? throw new NotFoundException($"Orden {idOrdenServicio} no encontrada.");

        if (!await ClienteEsPropietarioActualVehiculoAsync(cliente.IdCliente, orden.IdVehiculo))
            throw new BusinessRuleException("No tienes acceso a esa orden.");

        if (orden.CostoPropuesto is null)
            throw new BusinessRuleException("Esta orden no tiene un costo propuesto para aprobar/rechazar.");

        if (orden.CostoAprobado is not null)
            throw new BusinessRuleException("Esta orden ya fue aprobada/rechazada previamente.");

        orden.CostoAprobado = dto.Aprobado;
        orden.FechaDecisionCosto = DateTime.UtcNow;
        orden.ComentarioCliente = string.IsNullOrWhiteSpace(dto.Comentario) ? null : dto.Comentario.Trim();
        uow.OrdenesServicio.Update(orden);
        await uow.CommitAsync();
    }

    private async Task<Cliente> ObtenerClientePorUsuarioAsync(int idUsuario)
    {
        var usuario = await uow.Usuarios.GetByIdAsync(idUsuario)
            ?? throw new NotFoundException("Usuario no encontrado.");

        var clientes = await uow.Clientes.FindAsync(c => c.IdPersona == usuario.IdPersona && c.Estado);
        var cliente = clientes.FirstOrDefault();
        if (cliente is null)
            throw new BusinessRuleException("Tu cuenta no está asociada a un cliente.");

        return cliente;
    }

    private async Task<bool> ClienteEsPropietarioActualVehiculoAsync(int idCliente, int idVehiculo)
    {
        var historiales = await uow.HistorialPropietarios.FindAsync(h =>
            h.IdCliente == idCliente && h.IdVehiculo == idVehiculo && h.FechaFin == null);
        return historiales.Any();
    }
}

