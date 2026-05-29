using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services;

public class DiagnosticoService(IUnitOfWork uow, IMapper mapper, IAuditoriaService auditoria) : IDiagnosticoService
{
    public async Task<DiagnosticoDto?> ObtenerAsync(int idOrdenServicio)
    {
        var diag = (await uow.DiagnosticosOrden.FindAsync(d => d.IdOrdenServicio == idOrdenServicio))
            .FirstOrDefault();
        return diag is null ? null : mapper.Map<DiagnosticoDto>(diag);
    }

    public async Task<DiagnosticoDto> UpsertAsync(int idOrdenServicio, int idMecanico, UpsertDiagnosticoDto dto)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrdenServicio)
            ?? throw new NotFoundException($"Orden {idOrdenServicio} no encontrada.");

        if (orden.IdMecanico != idMecanico)
            throw new BusinessRuleException("Solo el mecánico asignado puede registrar el diagnóstico.");

        if (!await uow.Usuarios.TieneEspecializacionPorCodigoAsync(idMecanico, EspecializacionesMecanico.Diagnostico))
            throw new BusinessRuleException("Solo un mecánico con especialización en Diagnóstico puede registrar el diagnóstico.");

        // Regla: el diagnóstico solo se permite en etapa temprana.
        // Una vez que alguna reparación inició (EnProceso/Terminado), ya no se puede crear/editar diagnóstico.
        var estadoOrden = await uow.EstadosOrden.GetByIdAsync(orden.IdEstadoOrden);
        var estadoNombre = estadoOrden?.Nombre ?? string.Empty;
        if (estadoNombre is EstadosOrden.ReparacionEnProceso
            or EstadosOrden.ListoParaEntrega
            or EstadosOrden.PendientePago
            or EstadosOrden.Pagado
            or EstadosOrden.Entregado
            or EstadosOrden.Cerrado
            or EstadosOrden.Completada
            or EstadosOrden.Cancelada)
        {
            throw new BusinessRuleException("No se puede editar el diagnóstico: la orden ya está en etapa de reparación/entrega.");
        }

        var reparaciones = await uow.ReparacionesItem.FindAsync(r => r.IdOrdenServicio == idOrdenServicio);
        if (reparaciones.Any(r => r.Estado is EstadosReparacionItem.EnProceso or EstadosReparacionItem.Terminado))
            throw new BusinessRuleException("No se puede editar el diagnóstico: ya se inició la reparación.");

        var diag = (await uow.DiagnosticosOrden.FindAsync(d => d.IdOrdenServicio == idOrdenServicio))
            .FirstOrDefault();

        var esCreacion = diag is null;
        if (esCreacion)
        {
            diag = new DiagnosticoOrden
            {
                IdOrdenServicio = idOrdenServicio,
                IdMecanico = idMecanico,
                FechaDiagnostico = DateTime.UtcNow
            };
            mapper.Map(dto, diag);
            await uow.DiagnosticosOrden.AddAsync(diag);
        }
        else
        {
            mapper.Map(dto, diag);
            diag.FechaDiagnostico = DateTime.UtcNow;
            uow.DiagnosticosOrden.Update(diag);
        }

        await OrdenEstadoSync.SincronizarTrasDiagnosticoAsync(uow, orden);
        await uow.CommitAsync();

        await auditoria.RegistrarAsync(
            idMecanico,
            esCreacion ? "Creación" : "Actualización",
            nameof(DiagnosticoOrden),
            diag.IdDiagnosticoOrden,
            esCreacion ? "Diagnóstico registrado" : "Diagnóstico actualizado");

        return mapper.Map<DiagnosticoDto>(diag);
    }

    public async Task<IReadOnlyList<ReparacionItemDto>> ListarReparacionesAsync(int idOrdenServicio)
    {
        var items = await uow.ReparacionesItem.ListarPorOrdenConDetalleAsync(idOrdenServicio);
        return items.Select(mapper.Map<ReparacionItemDto>).ToList();
    }

    public async Task<ReparacionItemDto> CrearReparacionAsync(int idOrdenServicio, int idMecanico, CreateReparacionItemDto dto)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrdenServicio)
            ?? throw new NotFoundException($"Orden {idOrdenServicio} no encontrada.");

        if (orden.IdMecanico != idMecanico)
            throw new BusinessRuleException("Solo el mecánico asignado puede proponer reparaciones.");

        if (!await uow.Usuarios.TieneEspecializacionPorCodigoAsync(idMecanico, EspecializacionesMecanico.Diagnostico))
            throw new BusinessRuleException("Solo un mecánico con especialización en Diagnóstico puede proponer reparaciones.");

        var especializacion = await uow.EspecializacionesMecanico.GetByIdAsync(dto.IdEspecializacionMecanico);
        if (especializacion is null || !especializacion.Activo)
            throw new BusinessRuleException("La especialización requerida para la reparación no es válida.");

        var existentes = await uow.ReparacionesItem.FindAsync(r => r.IdOrdenServicio == idOrdenServicio);
        var nextOrden = existentes.Any() ? existentes.Max(r => r.Orden) + 1 : 1;

        var item = mapper.Map<ReparacionItem>(dto);
        item.IdOrdenServicio = idOrdenServicio;
        item.IdMecanico = idMecanico;
        item.Orden = nextOrden;
        item.Estado = EstadosReparacionItem.PendienteAprobacionJefe;

        await uow.ReparacionesItem.AddAsync(item);
        await OrdenEstadoSync.SincronizarTrasReparacionPropuestaAsync(uow, orden);
        await uow.CommitAsync();

        await auditoria.RegistrarAsync(
            idMecanico,
            "Creación",
            nameof(ReparacionItem),
            item.IdReparacionItem,
            "Reparación propuesta");

        var detalle = await uow.ReparacionesItem.GetDetalleAsync(item.IdReparacionItem);
        return mapper.Map<ReparacionItemDto>(detalle!);
    }

    public async Task<ReparacionItemDto> ActualizarReparacionAsync(int idOrdenServicio, int idReparacionItem, int idMecanico, UpdateReparacionItemDto dto)
    {
        var item = await uow.ReparacionesItem.GetDetalleAsync(idReparacionItem)
            ?? throw new NotFoundException($"Reparación {idReparacionItem} no encontrada.");

        if (item.IdOrdenServicio != idOrdenServicio)
            throw new BusinessRuleException("La reparación no pertenece a la orden indicada.");

        if (item.IdMecanico != idMecanico)
            throw new BusinessRuleException("Solo el mecánico que la propuso puede editarla.");

        if (item.Estado is EstadosReparacionItem.AprobadoCliente or EstadosReparacionItem.RechazadoCliente or EstadosReparacionItem.EnProceso or EstadosReparacionItem.Terminado)
            throw new BusinessRuleException("No se puede editar una reparación luego de la decisión del cliente.");

        var especializacion = await uow.EspecializacionesMecanico.GetByIdAsync(dto.IdEspecializacionMecanico);
        if (especializacion is null || !especializacion.Activo)
            throw new BusinessRuleException("La especialización requerida para la reparación no es válida.");

        mapper.Map(dto, item);
        uow.ReparacionesItem.Update(item);
        await uow.CommitAsync();

        await auditoria.RegistrarAsync(
            idMecanico,
            "Actualización",
            nameof(ReparacionItem),
            item.IdReparacionItem,
            "Reparación actualizada");

        var detalle = await uow.ReparacionesItem.GetDetalleAsync(item.IdReparacionItem);
        return mapper.Map<ReparacionItemDto>(detalle!);
    }

    public async Task<ReparacionItemDto> DecidirJefeAsync(int idOrdenServicio, int idReparacionItem, int idJefeUsuario, DecisionJefeDto dto)
    {
        var item = await uow.ReparacionesItem.GetDetalleAsync(idReparacionItem)
            ?? throw new NotFoundException($"Reparación {idReparacionItem} no encontrada.");

        if (item.IdOrdenServicio != idOrdenServicio)
            throw new BusinessRuleException("La reparación no pertenece a la orden indicada.");

        if (item.Estado != EstadosReparacionItem.PendienteAprobacionJefe)
            throw new BusinessRuleException("La reparación no está pendiente de aprobación del jefe.");

        item.AprobadoPorJefe = dto.Aprobar;
        item.FechaDecisionJefe = DateTime.UtcNow;
        item.ComentarioJefe = dto.Comentario;
        item.Estado = dto.Aprobar ? EstadosReparacionItem.PendienteDecisionCliente : EstadosReparacionItem.RechazadoJefe;

        uow.ReparacionesItem.Update(item);

        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrdenServicio)
            ?? throw new NotFoundException($"Orden {idOrdenServicio} no encontrada.");
        await OrdenEstadoSync.SincronizarTrasDecisionJefeAsync(uow, orden);
        await uow.CommitAsync();

        await auditoria.RegistrarAsync(
            idJefeUsuario,
            "Actualización",
            nameof(ReparacionItem),
            item.IdReparacionItem,
            "Decisión de jefe registrada");

        return mapper.Map<ReparacionItemDto>(item);
    }

    public async Task<IReadOnlyList<ReparacionItemDto>> DecidirClienteAsync(
        int idOrdenServicio,
        int idClienteUsuario,
        Dictionary<int, DecisionClienteDto> decisiones)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrdenServicio)
            ?? throw new NotFoundException($"Orden {idOrdenServicio} no encontrada.");

        await ValidarClienteOrdenAsync(idClienteUsuario, orden);

        var items = (await uow.ReparacionesItem.FindAsync(r => r.IdOrdenServicio == idOrdenServicio))
            .ToList();

        foreach (var item in items)
        {
            if (item.Estado != EstadosReparacionItem.PendienteDecisionCliente)
                continue;

            if (!decisiones.TryGetValue(item.IdReparacionItem, out var decision))
                continue;

            item.AprobadoPorCliente = decision.Aprobar;
            item.FechaDecisionCliente = DateTime.UtcNow;
            item.ComentarioCliente = decision.Comentario;
            item.Estado = decision.Aprobar ? EstadosReparacionItem.AprobadoCliente : EstadosReparacionItem.RechazadoCliente;
            uow.ReparacionesItem.Update(item);
        }

        await OrdenEstadoSync.SincronizarTrasDecisionClienteAsync(uow, orden);
        await uow.CommitAsync();

        await auditoria.RegistrarAsync(
            idClienteUsuario,
            "Actualización",
            nameof(OrdenServicio),
            idOrdenServicio,
            "Decisión del cliente registrada (parcial/total)");

        var detalle = await uow.ReparacionesItem.ListarPorOrdenConDetalleAsync(idOrdenServicio);
        return detalle.Select(mapper.Map<ReparacionItemDto>).ToList();
    }

    public async Task<IReadOnlyList<ReparacionItemDto>> ListarPendientesJefeAsync(int page, int size)
    {
        page = page < 1 ? 1 : page;
        size = size is < 1 or > 200 ? 50 : size;

        var items = await uow.ReparacionesItem.ListarPendientesJefeConDetalleAsync(page, size);
        return items.Select(mapper.Map<ReparacionItemDto>).ToList();
    }

    public async Task<ReparacionItemDto> IniciarReparacionAsync(int idOrdenServicio, int idReparacionItem, int idMecanico)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrdenServicio)
            ?? throw new NotFoundException($"Orden {idOrdenServicio} no encontrada.");

        var item = await uow.ReparacionesItem.GetDetalleAsync(idReparacionItem)
            ?? throw new NotFoundException($"Reparación {idReparacionItem} no encontrada.");

        if (item.IdOrdenServicio != idOrdenServicio)
            throw new BusinessRuleException("La reparación no pertenece a la orden indicada.");

        if (item.Estado != EstadosReparacionItem.AprobadoCliente)
            throw new BusinessRuleException("Solo se puede iniciar una reparación aprobada por el cliente.");

        if (!await uow.Usuarios.TieneEspecializacionAsync(idMecanico, item.IdEspecializacionMecanico))
            throw new BusinessRuleException("No tienes la especialización requerida para esta reparación.");

        item.IdMecanico = idMecanico;
        item.Estado = EstadosReparacionItem.EnProceso;
        item.FechaInicio = DateTime.UtcNow;
        uow.ReparacionesItem.Update(item);
        await OrdenEstadoSync.SincronizarTrasIniciarReparacionAsync(uow, orden);
        await uow.CommitAsync();

        await auditoria.RegistrarAsync(
            idMecanico,
            "Actualización",
            nameof(ReparacionItem),
            item.IdReparacionItem,
            "Reparación iniciada");

        var detalle = await uow.ReparacionesItem.GetDetalleAsync(item.IdReparacionItem);
        return mapper.Map<ReparacionItemDto>(detalle!);
    }

    public async Task<ReparacionItemDto> TerminarReparacionAsync(int idOrdenServicio, int idReparacionItem, int idMecanico)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrdenServicio)
            ?? throw new NotFoundException($"Orden {idOrdenServicio} no encontrada.");

        var item = await uow.ReparacionesItem.GetDetalleAsync(idReparacionItem)
            ?? throw new NotFoundException($"Reparación {idReparacionItem} no encontrada.");

        if (item.IdOrdenServicio != idOrdenServicio)
            throw new BusinessRuleException("La reparación no pertenece a la orden indicada.");

        if (item.IdMecanico != idMecanico)
            throw new BusinessRuleException("Solo el mecánico que inició la reparación puede terminarla.");

        if (item.Estado != EstadosReparacionItem.EnProceso)
            throw new BusinessRuleException("Solo se puede terminar una reparación en proceso.");

        item.Estado = EstadosReparacionItem.Terminado;
        item.FechaFin = DateTime.UtcNow;
        uow.ReparacionesItem.Update(item);
        await OrdenEstadoSync.SincronizarTrasTerminarReparacionAsync(uow, orden);
        await uow.CommitAsync();

        await auditoria.RegistrarAsync(
            idMecanico,
            "Actualización",
            nameof(ReparacionItem),
            item.IdReparacionItem,
            "Reparación terminada");

        var detalle = await uow.ReparacionesItem.GetDetalleAsync(item.IdReparacionItem);
        return mapper.Map<ReparacionItemDto>(detalle!);
    }

    private async Task ValidarClienteOrdenAsync(int idClienteUsuario, OrdenServicio orden)
    {
        var usuario = await uow.Usuarios.GetByIdAsync(idClienteUsuario)
            ?? throw new NotFoundException("Usuario no encontrado.");

        var clientes = await uow.Clientes.FindAsync(c => c.IdPersona == usuario.IdPersona && c.Estado);
        var cliente = clientes.FirstOrDefault()
            ?? throw new BusinessRuleException("Tu cuenta no está asociada a un cliente.");

        if (orden.IdCliente != cliente.IdCliente)
            throw new BusinessRuleException("No tienes acceso a esa orden.");
    }
}

