using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services;

public class OrdenServicioService(
    IUnitOfWork uow,
    IMapper mapper,
    IAuditoriaService auditoriaService) : IOrdenServicioService
{
    public async Task<OrdenServicioDto> CrearOrdenAsync(CreateOrdenServicioDto dto)
    {
        var vehiculo = await uow.Vehiculos.GetByIdAsync(dto.IdVehiculo)
            ?? throw new NotFoundException($"Vehículo {dto.IdVehiculo} no encontrado.");

        var ordenesActivas = await uow.OrdenesServicio.FindAsync(o => o.IdVehiculo == dto.IdVehiculo);
        foreach (var ordenExistente in ordenesActivas)
        {
            await CargarEstadoOrdenAsync(ordenExistente);
            if (ordenExistente.EstaActiva())
                throw new BusinessRuleException("El vehículo ya tiene una orden de servicio activa.");
        }

        var tipoServicio = await uow.TiposServicio.GetByIdAsync(dto.IdTipoServicio)
            ?? throw new NotFoundException($"Tipo de servicio {dto.IdTipoServicio} no encontrado.");

        var estadoPendiente = (await uow.EstadosOrden.FindAsync(e => e.Nombre == EstadosOrden.Pendiente)).FirstOrDefault()
            ?? throw new NotFoundException("Estado de orden 'Pendiente' no configurado.");

        var orden = new OrdenServicio
        {
            IdVehiculo = dto.IdVehiculo,
            IdTipoServicio = dto.IdTipoServicio,
            IdMecanico = dto.IdMecanico,
            IdEstadoOrden = estadoPendiente.IdEstadoOrden,
            FechaIngreso = DateTime.UtcNow,
            FechaEstimadaEntrega = CalcularFechaEstimada(tipoServicio.Nombre)
        };

        await uow.OrdenesServicio.AddAsync(orden);
        await uow.CommitAsync();

        await auditoriaService.RegistrarAsync(
            dto.IdMecanico,
            "Creación",
            nameof(OrdenServicio),
            orden.IdOrdenServicio,
            "Orden de servicio creada");

        await CargarRelacionesOrdenAsync(orden);
        return mapper.Map<OrdenServicioDto>(orden);
    }

    public async Task ActualizarConTrabajoRealizadoAsync(int id, UpdateOrdenServicioDto dto)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(id)
            ?? throw new NotFoundException($"Orden de servicio {id} no encontrada.");

        orden.IdEstadoOrden = dto.IdEstadoOrden;
        orden.TrabajoRealizado = dto.TrabajoRealizado;
        orden.FechaEstimadaEntrega = dto.FechaEstimadaEntrega;
        uow.OrdenesServicio.Update(orden);

        var detalles = await uow.DetallesOrden.FindAsync(d => d.IdOrdenServicio == id);
        foreach (var detalle in detalles)
        {
            var repuesto = await uow.Repuestos.GetByIdAsync(detalle.IdRepuesto)
                ?? throw new NotFoundException($"Repuesto {detalle.IdRepuesto} no encontrado.");

            if (!repuesto.TieneStock(detalle.Cantidad))
                throw new BusinessRuleException($"Stock insuficiente para el repuesto {repuesto.Codigo}.");

            repuesto.Stock -= detalle.Cantidad;
            uow.Repuestos.Update(repuesto);
        }

        await uow.CommitAsync();

        await auditoriaService.RegistrarAsync(
            orden.IdMecanico,
            "Actualización",
            nameof(OrdenServicio),
            orden.IdOrdenServicio,
            "Orden actualizada con trabajo realizado");
    }

    public async Task CancelarOrdenAsync(int id)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(id)
            ?? throw new NotFoundException($"Orden de servicio {id} no encontrada.");

        var estadoCancelada = (await uow.EstadosOrden.FindAsync(e => e.Nombre == EstadosOrden.Cancelada)).FirstOrDefault()
            ?? throw new NotFoundException("Estado de orden 'Cancelada' no configurado.");

        orden.IdEstadoOrden = estadoCancelada.IdEstadoOrden;
        uow.OrdenesServicio.Update(orden);
        await uow.CommitAsync();

        await auditoriaService.RegistrarAsync(
            orden.IdMecanico,
            "Actualización",
            nameof(OrdenServicio),
            orden.IdOrdenServicio,
            "Orden cancelada");
    }

    public async Task<PagedResultDto<OrdenServicioDto>> ListarAsync(
        int page,
        int size,
        string? estado,
        int? idCliente,
        int? idMecanico,
        DateTime? desde,
        DateTime? hasta)
    {
        HashSet<int>? idsVehiculos = null;
        if (idCliente is not null)
        {
            var historiales = await uow.HistorialPropietarios.FindAsync(h => h.IdCliente == idCliente);
            idsVehiculos = historiales.Select(h => h.IdVehiculo).ToHashSet();
        }

        var (items, total) = await uow.OrdenesServicio.GetPagedAsync(page, size, o =>
            (idMecanico == null || o.IdMecanico == idMecanico) &&
            (desde == null || o.FechaIngreso >= desde) &&
            (hasta == null || o.FechaIngreso <= hasta) &&
            (idsVehiculos == null || idsVehiculos.Contains(o.IdVehiculo)) &&
            (estado == null || (o.EstadoOrden != null && o.EstadoOrden.Nombre == estado)));

        var dtos = new List<OrdenServicioDto>();
        foreach (var orden in items)
        {
            await CargarRelacionesOrdenAsync(orden);
            dtos.Add(mapper.Map<OrdenServicioDto>(orden));
        }

        return new PagedResultDto<OrdenServicioDto>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = page,
            PageSize = size
        };
    }

    public async Task<OrdenServicioDto?> ObtenerPorIdAsync(int id)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(id);
        if (orden is null)
            return null;

        await CargarRelacionesOrdenAsync(orden);
        return mapper.Map<OrdenServicioDto>(orden);
    }

    private static DateTime CalcularFechaEstimada(string nombreTipoServicio) =>
        nombreTipoServicio switch
        {
            "Reparación" => DateTime.UtcNow.AddDays(3),
            _ => DateTime.UtcNow.AddDays(1)
        };

    private async Task CargarEstadoOrdenAsync(OrdenServicio orden)
    {
        if (orden.EstadoOrden is null)
            orden.EstadoOrden = await uow.EstadosOrden.GetByIdAsync(orden.IdEstadoOrden);
    }

    private async Task CargarRelacionesOrdenAsync(OrdenServicio orden)
    {
        orden.Vehiculo ??= await uow.Vehiculos.GetByIdAsync(orden.IdVehiculo);
        orden.TipoServicio ??= await uow.TiposServicio.GetByIdAsync(orden.IdTipoServicio);
        orden.Mecanico ??= await uow.Usuarios.GetByIdAsync(orden.IdMecanico);
        if (orden.Mecanico?.Persona is null && orden.Mecanico is not null)
            orden.Mecanico.Persona = await uow.Personas.GetByIdAsync(orden.Mecanico.IdPersona);
        await CargarEstadoOrdenAsync(orden);
    }
}
