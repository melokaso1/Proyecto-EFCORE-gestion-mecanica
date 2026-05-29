using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;

namespace Application.Services;

internal static class OrdenEstadoSync
{
    public static async Task SincronizarTrasDiagnosticoAsync(IUnitOfWork uow, OrdenServicio orden)
    {
        var actual = await ObtenerNombreEstadoAsync(uow, orden);
        if (actual is EstadosOrden.Recibido or EstadosOrden.Pendiente)
            await CambiarEstadoAsync(uow, orden, EstadosOrden.DiagnosticoEnProceso);
    }

    public static async Task SincronizarTrasReparacionPropuestaAsync(IUnitOfWork uow, OrdenServicio orden)
    {
        var items = (await uow.ReparacionesItem.FindAsync(r => r.IdOrdenServicio == orden.IdOrdenServicio)).ToList();
        if (items.Any(i => i.Estado == EstadosReparacionItem.PendienteAprobacionJefe))
            await CambiarEstadoAsync(uow, orden, EstadosOrden.PendienteAprobacionJefe);
    }

    public static async Task SincronizarTrasDecisionJefeAsync(IUnitOfWork uow, OrdenServicio orden)
    {
        var items = (await uow.ReparacionesItem.FindAsync(r => r.IdOrdenServicio == orden.IdOrdenServicio)).ToList();
        if (items.Count == 0)
            return;

        if (items.Any(i => i.Estado == EstadosReparacionItem.PendienteAprobacionJefe))
        {
            await CambiarEstadoAsync(uow, orden, EstadosOrden.PendienteAprobacionJefe);
            return;
        }

        if (items.Any(i => i.Estado == EstadosReparacionItem.PendienteDecisionCliente))
        {
            await CambiarEstadoAsync(uow, orden, EstadosOrden.PendienteAprobacionCliente);
            return;
        }

        if (items.All(i => i.Estado is EstadosReparacionItem.RechazadoJefe or EstadosReparacionItem.RechazadoCliente))
            await CambiarEstadoAsync(uow, orden, EstadosOrden.RechazadoCliente);
    }

    public static async Task SincronizarTrasDecisionClienteAsync(IUnitOfWork uow, OrdenServicio orden)
    {
        var items = (await uow.ReparacionesItem.FindAsync(r => r.IdOrdenServicio == orden.IdOrdenServicio)).ToList();
        if (items.Any(i => i.Estado == EstadosReparacionItem.PendienteDecisionCliente))
        {
            await CambiarEstadoAsync(uow, orden, EstadosOrden.PendienteAprobacionCliente);
            return;
        }

        var aprobados = items.Where(i => i.Estado == EstadosReparacionItem.AprobadoCliente).ToList();
        if (aprobados.Count == 0)
        {
            await CambiarEstadoAsync(uow, orden, EstadosOrden.RechazadoCliente);
            return;
        }

        var rechazados = items.Count(i => i.Estado == EstadosReparacionItem.RechazadoCliente);
        var destino = rechazados > 0 ? EstadosOrden.AprobadoParcial : EstadosOrden.AprobadoTotal;
        await CambiarEstadoAsync(uow, orden, destino);
    }

    public static async Task SincronizarTrasIniciarReparacionAsync(IUnitOfWork uow, OrdenServicio orden) =>
        await CambiarEstadoAsync(uow, orden, EstadosOrden.ReparacionEnProceso);

    public static async Task SincronizarTrasTerminarReparacionAsync(IUnitOfWork uow, OrdenServicio orden)
    {
        var items = (await uow.ReparacionesItem.FindAsync(r => r.IdOrdenServicio == orden.IdOrdenServicio)).ToList();
        var aprobados = items.Where(i => i.Estado is EstadosReparacionItem.AprobadoCliente
            or EstadosReparacionItem.EnProceso
            or EstadosReparacionItem.Terminado).ToList();

        if (aprobados.Count == 0)
            return;

        if (aprobados.All(i => i.Estado == EstadosReparacionItem.Terminado))
            await CambiarEstadoAsync(uow, orden, EstadosOrden.PendientePago);
    }

    private static async Task<string?> ObtenerNombreEstadoAsync(IUnitOfWork uow, OrdenServicio orden)
    {
        if (orden.EstadoOrden?.Nombre is not null)
            return orden.EstadoOrden.Nombre;

        orden.EstadoOrden = await uow.EstadosOrden.GetByIdAsync(orden.IdEstadoOrden);
        return orden.EstadoOrden?.Nombre;
    }

    private static async Task CambiarEstadoAsync(IUnitOfWork uow, OrdenServicio orden, string nombreEstado)
    {
        var actual = await ObtenerNombreEstadoAsync(uow, orden);
        if (actual == nombreEstado)
            return;

        var estado = (await uow.EstadosOrden.FindAsync(e => e.Nombre == nombreEstado)).FirstOrDefault();
        if (estado is null)
            return;

        orden.IdEstadoOrden = estado.IdEstadoOrden;
        orden.EstadoOrden = estado;
        uow.OrdenesServicio.Update(orden);
    }
}
