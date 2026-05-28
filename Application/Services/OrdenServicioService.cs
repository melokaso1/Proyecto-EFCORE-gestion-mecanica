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
        var cliente = await uow.Clientes.GetByIdAsync(dto.IdCliente)
            ?? throw new NotFoundException($"Cliente {dto.IdCliente} no encontrado.");

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
            IdCliente = cliente.IdCliente,
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
        var (items, total) = await uow.OrdenesServicio.GetPagedConFiltrosAsync(
            page, size, estado, idCliente, idMecanico, desde, hasta);

        var dtos = new List<OrdenServicioDto>();
        foreach (var orden in items)
        {
            await CargarRelacionesOrdenAsync(orden);
            var dto = mapper.Map<OrdenServicioDto>(orden);
            dto.ClienteNombre = orden.Cliente?.Persona is null
                ? string.Empty
                : $"{orden.Cliente.Persona.Nombres} {orden.Cliente.Persona.Apellidos}".Trim();
            dtos.Add(dto);
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
        var dto = mapper.Map<OrdenServicioDto>(orden);
        dto.ClienteNombre = orden.Cliente?.Persona is null
            ? string.Empty
            : $"{orden.Cliente.Persona.Nombres} {orden.Cliente.Persona.Apellidos}".Trim();
        return dto;
    }

    public async Task<SeguimientoOrdenDto?> ConsultarSeguimientoAsync(
        string documento,
        string? vin,
        int? codigoOrden)
    {
        if (string.IsNullOrWhiteSpace(documento))
            return null;

        var docs = await uow.DocumentosPersona.FindAsync(d =>
            d.NumeroDocumento == documento.Trim() && d.EsPrincipal);
        var doc = docs.FirstOrDefault();
        if (doc is null)
            return null;

        var clientes = await uow.Clientes.FindAsync(c => c.IdPersona == doc.IdPersona);
        var cliente = clientes.FirstOrDefault();
        if (cliente is null)
            return null;

        OrdenServicio? orden;

        if (codigoOrden is not null)
        {
            orden = await uow.OrdenesServicio.GetByIdAsync(codigoOrden.Value);
            if (orden is null)
                return null;

            if (orden.IdCliente != cliente.IdCliente)
                return null;
        }
        else if (!string.IsNullOrWhiteSpace(vin))
        {
            var vehiculo = await uow.Vehiculos.ObtenerPorVinAsync(vin.Trim());
            if (vehiculo is null)
                return null;

            var ordenes = await uow.OrdenesServicio.FindAsync(o => o.IdVehiculo == vehiculo.IdVehiculo && o.IdCliente == cliente.IdCliente);
            orden = ordenes.OrderByDescending(o => o.FechaIngreso).FirstOrDefault();
            if (orden is null)
                return null;
        }
        else
        {
            return null;
        }

        await CargarRelacionesOrdenAsync(orden);
        var persona = await uow.Personas.GetByIdAsync(doc.IdPersona);

        return new SeguimientoOrdenDto
        {
            IdOrdenServicio = orden.IdOrdenServicio,
            Estado = orden.EstadoOrden?.Nombre ?? string.Empty,
            VIN = orden.Vehiculo?.VIN ?? string.Empty,
            MarcaModelo = orden.Vehiculo?.Modelo == null
                ? string.Empty
                : $"{orden.Vehiculo.Modelo.Marca?.NombreMarca} {orden.Vehiculo.Modelo.NombreModelo}".Trim(),
            Anio = orden.Vehiculo?.Anio ?? 0,
            TipoServicio = orden.TipoServicio?.Nombre ?? string.Empty,
            ClienteNombre = persona == null
                ? string.Empty
                : $"{persona.Nombres} {persona.Apellidos}".Trim(),
            FechaIngreso = orden.FechaIngreso,
            FechaEstimadaEntrega = orden.FechaEstimadaEntrega,
            TrabajoRealizado = orden.TrabajoRealizado
        };
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
        orden.Cliente ??= await uow.Clientes.GetByIdAsync(orden.IdCliente);
        if (orden.Cliente?.Persona is null && orden.Cliente is not null)
            orden.Cliente.Persona = await uow.Personas.GetByIdAsync(orden.Cliente.IdPersona);
        orden.TipoServicio ??= await uow.TiposServicio.GetByIdAsync(orden.IdTipoServicio);
        orden.Mecanico ??= await uow.Usuarios.GetByIdAsync(orden.IdMecanico);
        if (orden.Mecanico?.Persona is null && orden.Mecanico is not null)
            orden.Mecanico.Persona = await uow.Personas.GetByIdAsync(orden.Mecanico.IdPersona);
        await CargarEstadoOrdenAsync(orden);
    }
}
