using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;

namespace Application.Services;

public class CasoRecepcionService(
    IUnitOfWork uow,
    IClienteService clienteService,
    IVehiculoService vehiculoService,
    IAuditoriaService auditoria) : ICasoRecepcionService
{
    public async Task<CasoRecepcionDto> IniciarCasoAsync(int idRecepcionista)
    {
        var estado = await ObtenerEstadoAsync(EstadosOrden.EnRegistro);

        var orden = new OrdenServicio
        {
            IdEstadoOrden = estado.IdEstadoOrden,
            IdRecepcionista = idRecepcionista,
            FechaIngreso = DateTime.UtcNow
        };

        await uow.OrdenesServicio.AddAsync(orden);
        await uow.CommitAsync();

        await auditoria.RegistrarAsync(
            idRecepcionista,
            "Creación",
            nameof(OrdenServicio),
            orden.IdOrdenServicio,
            "Caso de recepción iniciado");

        return await MapCasoAsync(orden);
    }

    public async Task<CasoRecepcionDto?> ObtenerCasoAsync(int idOrden)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrden);
        return orden is null ? null : await MapCasoAsync(orden);
    }

    public async Task<PagedResultDto<CasoRecepcionDto>> ListarCasosEnRegistroAsync(int page, int size)
    {
        var result = await uow.OrdenesServicio.GetPagedConFiltrosAsync(
            page, size, EstadosOrden.EnRegistro, null, null, null, null);

        var items = new List<CasoRecepcionDto>();
        foreach (var orden in result.items)
            items.Add(await MapCasoAsync(orden));

        return new PagedResultDto<CasoRecepcionDto>
        {
            Items = items,
            TotalCount = result.total,
            PageNumber = page,
            PageSize = size
        };
    }

    public Task<ClienteDto?> BuscarClientePorDocumentoAsync(string numeroDocumento) =>
        clienteService.BuscarPorDocumentoAsync(numeroDocumento);

    public Task<VehiculoDto?> BuscarVehiculoPorPlacaAsync(string placa) =>
        vehiculoService.ObtenerPorPlacaAsync(placa);

    public Task<VehiculoDto?> BuscarVehiculoPorVinAsync(string vin) =>
        vehiculoService.ObtenerPorVinAsync(vin);

    public async Task<CasoRecepcionDto> AsignarClienteAsync(int idOrden, AsignarClienteCasoDto dto)
    {
        var orden = await ObtenerCasoEnRegistroAsync(idOrden);

        if (dto.IdCliente is > 0)
        {
            var cliente = await uow.Clientes.GetByIdAsync(dto.IdCliente.Value)
                ?? throw new NotFoundException($"Cliente {dto.IdCliente} no encontrado.");
            if (!cliente.Estado)
                throw new BusinessRuleException("El cliente está inactivo.");
            orden.IdCliente = cliente.IdCliente;
        }
        else if (dto.DatosNuevoCliente is not null)
        {
            var creado = await clienteService.RegistrarClienteAsync(dto.DatosNuevoCliente);
            orden.IdCliente = creado.IdCliente;
        }
        else
        {
            throw new BusinessRuleException("Indica un cliente existente o los datos de uno nuevo.");
        }

        uow.OrdenesServicio.Update(orden);
        await uow.CommitAsync();

        return await MapCasoAsync(await uow.OrdenesServicio.GetByIdAsync(idOrden) ?? orden);
    }

    public async Task<CasoRecepcionDto> AsignarVehiculoAsync(int idOrden, AsignarVehiculoCasoDto dto)
    {
        var orden = await ObtenerCasoEnRegistroAsync(idOrden);

        if (dto.IdVehiculo is > 0)
        {
            var vehiculo = await uow.Vehiculos.GetByIdAsync(dto.IdVehiculo.Value)
                ?? throw new NotFoundException($"Vehículo {dto.IdVehiculo} no encontrado.");

            await ValidarVehiculoDisponibleParaCasoAsync(vehiculo.IdVehiculo, idOrden);
            orden.IdVehiculo = vehiculo.IdVehiculo;
        }
        else if (dto.DatosNuevoVehiculo is not null)
        {
            var creado = await vehiculoService.CrearEnCatalogoAsync(dto.DatosNuevoVehiculo);

            if (orden.IdCliente is > 0)
            {
                var yaTieneHistorial = (await uow.HistorialPropietarios
                    .FindAsync(h => h.IdVehiculo == creado.IdVehiculo && h.FechaFin == null)).Any();

                if (!yaTieneHistorial)
                {
                    await uow.HistorialPropietarios.AddAsync(new HistorialPropietarioVehiculo
                    {
                        IdVehiculo = creado.IdVehiculo,
                        IdCliente = orden.IdCliente.Value,
                        FechaInicio = DateTime.UtcNow
                    });
                }
            }

            orden.IdVehiculo = creado.IdVehiculo;
        }
        else
        {
            throw new BusinessRuleException("Indica un vehículo existente o los datos de uno nuevo.");
        }

        uow.OrdenesServicio.Update(orden);
        await uow.CommitAsync();

        return await MapCasoAsync(await uow.OrdenesServicio.GetByIdAsync(idOrden) ?? orden);
    }

    public async Task<CasoRecepcionDto> ConfirmarCasoAsync(int idOrden, ConfirmarCasoDto dto)
    {
        var orden = await ObtenerCasoEnRegistroAsync(idOrden);

        if (orden.IdCliente is null or <= 0)
            throw new BusinessRuleException("Debes asignar el dueño del vehículo al caso.");
        if (orden.IdVehiculo is null or <= 0)
            throw new BusinessRuleException("Debes asignar el vehículo al caso.");

        var tipoServicio = await uow.TiposServicio.GetByIdAsync(dto.IdTipoServicio)
            ?? throw new NotFoundException($"Tipo de servicio {dto.IdTipoServicio} no encontrado.");

        var mecanico = await uow.Usuarios.GetByIdAsync(dto.IdMecanico)
            ?? throw new NotFoundException($"Mecánico {dto.IdMecanico} no encontrado.");

        if (!mecanico.Roles.Any(r => r.NombreRol == "Mecánico"))
            throw new BusinessRuleException("El usuario seleccionado no es mecánico.");

        await ValidarVehiculoDisponibleParaCasoAsync(orden.IdVehiculo.Value, idOrden);

        var estadoRecibido = await ObtenerEstadoAsync(EstadosOrden.Recibido);
        orden.IdTipoServicio = dto.IdTipoServicio;
        orden.IdMecanico = dto.IdMecanico;
        orden.MotivoIngreso = string.IsNullOrWhiteSpace(dto.MotivoIngreso) ? null : dto.MotivoIngreso.Trim();
        orden.IdEstadoOrden = estadoRecibido.IdEstadoOrden;
        orden.FechaEstimadaEntrega = CalcularFechaEstimada(tipoServicio.Nombre);

        uow.OrdenesServicio.Update(orden);
        await uow.CommitAsync();

        var idActor = orden.IdRecepcionista ?? dto.IdMecanico;
        await auditoria.RegistrarAsync(
            idActor,
            "Actualización",
            nameof(OrdenServicio),
            orden.IdOrdenServicio,
            "Caso confirmado — vehículo recibido");

        return await MapCasoAsync(await uow.OrdenesServicio.GetByIdAsync(idOrden) ?? orden);
    }

    public async Task CancelarCasoAsync(int idOrden)
    {
        var orden = await ObtenerCasoEnRegistroAsync(idOrden);
        var estadoCancelada = await ObtenerEstadoAsync(EstadosOrden.Cancelada);
        orden.IdEstadoOrden = estadoCancelada.IdEstadoOrden;
        uow.OrdenesServicio.Update(orden);
        await uow.CommitAsync();
    }

    private async Task<OrdenServicio> ObtenerCasoEnRegistroAsync(int idOrden)
    {
        var orden = await uow.OrdenesServicio.GetByIdAsync(idOrden)
            ?? throw new NotFoundException($"Caso {idOrden} no encontrado.");

        if (orden.EstadoOrden?.Nombre != EstadosOrden.EnRegistro)
        {
            if (orden.EstadoOrden is null)
            {
                orden.EstadoOrden = await uow.EstadosOrden.GetByIdAsync(orden.IdEstadoOrden);
            }

            if (orden.EstadoOrden?.Nombre != EstadosOrden.EnRegistro)
                throw new BusinessRuleException("El caso ya no está en registro.");
        }

        return orden;
    }

    private async Task ValidarVehiculoDisponibleParaCasoAsync(int idVehiculo, int idOrdenExcluir)
    {
        var ordenes = await uow.OrdenesServicio.FindAsync(o => o.IdVehiculo == idVehiculo && o.IdOrdenServicio != idOrdenExcluir);
        foreach (var o in ordenes)
        {
            if (o.EstadoOrden is null)
                o.EstadoOrden = await uow.EstadosOrden.GetByIdAsync(o.IdEstadoOrden);

            if (o.EstadoOrden?.Nombre == EstadosOrden.EnRegistro)
                throw new BusinessRuleException("Ese vehículo ya está en otro caso en registro.");

            if (o.EstaActiva())
                throw new BusinessRuleException("El vehículo ya tiene una orden de servicio activa.");
        }
    }

    private async Task<EstadoOrden> ObtenerEstadoAsync(string nombre) =>
        (await uow.EstadosOrden.FindAsync(e => e.Nombre == nombre)).FirstOrDefault()
        ?? throw new NotFoundException($"Estado '{nombre}' no configurado.");

    private static DateTime CalcularFechaEstimada(string nombreTipoServicio) =>
        nombreTipoServicio switch
        {
            "Reparación" => DateTime.UtcNow.AddDays(3),
            _ => DateTime.UtcNow.AddDays(1)
        };

    private async Task<CasoRecepcionDto> MapCasoAsync(OrdenServicio orden)
    {
        if (orden.EstadoOrden is null)
            orden.EstadoOrden = await uow.EstadosOrden.GetByIdAsync(orden.IdEstadoOrden);

        if (orden.Cliente is null && orden.IdCliente is > 0)
            orden.Cliente = await uow.Clientes.GetByIdAsync(orden.IdCliente.Value);

        if (orden.Vehiculo is null && orden.IdVehiculo is > 0)
            orden.Vehiculo = await uow.Vehiculos.GetByIdAsync(orden.IdVehiculo.Value);

        if (orden.TipoServicio is null && orden.IdTipoServicio is > 0)
            orden.TipoServicio = await uow.TiposServicio.GetByIdAsync(orden.IdTipoServicio.Value);

        if (orden.Mecanico is null && orden.IdMecanico is > 0)
            orden.Mecanico = await uow.Usuarios.GetByIdAsync(orden.IdMecanico.Value);

        string? documento = null;
        if (orden.Cliente?.Persona is not null)
        {
            var docs = await uow.DocumentosPersona.FindAsync(d =>
                d.IdPersona == orden.Cliente.IdPersona && d.EsPrincipal);
            documento = docs.FirstOrDefault()?.NumeroDocumento;
        }

        return new CasoRecepcionDto
        {
            IdOrdenServicio = orden.IdOrdenServicio,
            Estado = orden.EstadoOrden?.Nombre ?? string.Empty,
            FechaIngreso = orden.FechaIngreso,
            MotivoIngreso = orden.MotivoIngreso,
            IdCliente = orden.IdCliente,
            ClienteNombre = orden.Cliente?.Persona is null
                ? null
                : $"{orden.Cliente.Persona.Nombres} {orden.Cliente.Persona.Apellidos}".Trim(),
            ClienteDocumento = documento,
            ClienteCorreo = orden.Cliente?.Persona?.CorreosPersona.FirstOrDefault(c => c.EsPrincipal) is { } cp
                ? $"{cp.UsuarioCorreo}@{cp.DominioCorreo?.Dominio ?? "local"}"
                : null,
            ClienteTelefono = orden.Cliente?.Persona?.TelefonosPersona.FirstOrDefault(t => t.EsPrincipal)?.NumeroTelefono,
            IdVehiculo = orden.IdVehiculo,
            VehiculoPlaca = orden.Vehiculo?.Placa,
            VehiculoVin = orden.Vehiculo?.VIN,
            VehiculoMarcaModelo = orden.Vehiculo?.Modelo is null
                ? null
                : $"{orden.Vehiculo.Modelo.Marca?.NombreMarca} {orden.Vehiculo.Modelo.NombreModelo}".Trim(),
            VehiculoAnio = orden.Vehiculo?.Anio,
            VehiculoKilometraje = orden.Vehiculo?.Kilometraje,
            IdTipoServicio = orden.IdTipoServicio,
            TipoServicio = orden.TipoServicio?.Nombre,
            IdMecanico = orden.IdMecanico,
            MecanicoNombre = orden.Mecanico?.Persona is null
                ? null
                : $"{orden.Mecanico.Persona.Nombres} {orden.Mecanico.Persona.Apellidos}".Trim()
        };
    }
}
