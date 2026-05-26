using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services;

public class ClienteService(IUnitOfWork uow, IMapper mapper) : IClienteService
{
    public async Task<ClienteDto> RegistrarClienteConVehiculoAsync(CreateClienteDto dto)
    {
        var persona = mapper.Map<Persona>(dto);
        await uow.Personas.AddAsync(persona);
        await uow.CommitAsync();

        var cliente = mapper.Map<Cliente>(dto);
        cliente.IdPersona = persona.IdPersona;
        await uow.Clientes.AddAsync(cliente);

        await uow.DocumentosPersona.AddAsync(new DocumentoPersona
        {
            IdPersona = persona.IdPersona,
            IdTipoDocumento = dto.IdTipoDocumento,
            NumeroDocumento = dto.NumeroDocumento,
            EsPrincipal = true
        });

        var (usuarioCorreo, dominio) = ParseCorreo(dto.Correo);
        var idDominio = await ObtenerOCrearDominioAsync(dominio);
        await uow.CorreosPersona.AddAsync(new CorreoPersona
        {
            IdPersona = persona.IdPersona,
            IdDominioCorreo = idDominio,
            UsuarioCorreo = usuarioCorreo,
            EsPrincipal = true
        });

        var idCodigo = await ObtenerOCrearCodigoTelefonoAsync();
        await uow.TelefonosPersona.AddAsync(new TelefonoPersona
        {
            IdPersona = persona.IdPersona,
            IdCodigoTelefono = idCodigo,
            NumeroTelefono = dto.Telefono,
            EsPrincipal = true
        });

        await uow.CommitAsync();

        cliente.Persona = persona;
        return mapper.Map<ClienteDto>(cliente);
    }

    public async Task<PagedResultDto<ClienteDto>> ListarClientesAsync(int page, int size, string? filtro)
    {
        var (items, total) = await uow.Clientes.GetPagedAsync(page, size, c =>
            filtro == null ||
            (c.Persona != null &&
             (c.Persona.Nombres.Contains(filtro) || c.Persona.Apellidos.Contains(filtro))));

        return new PagedResultDto<ClienteDto>
        {
            Items = mapper.Map<List<ClienteDto>>(items),
            TotalCount = total,
            PageNumber = page,
            PageSize = size
        };
    }

    public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
    {
        var cliente = await uow.Clientes.GetByIdAsync(id);
        return cliente is null ? null : mapper.Map<ClienteDto>(cliente);
    }

    public async Task ActualizarAsync(int id, CreateClienteDto dto)
    {
        var cliente = await uow.Clientes.GetByIdAsync(id)
            ?? throw new NotFoundException($"Cliente {id} no encontrado.");

        var persona = await uow.Personas.GetByIdAsync(cliente.IdPersona)
            ?? throw new NotFoundException($"Persona del cliente {id} no encontrada.");

        persona.Nombres = dto.Nombres;
        persona.Apellidos = dto.Apellidos;
        uow.Personas.Update(persona);
        await uow.CommitAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var cliente = await uow.Clientes.GetByIdAsync(id)
            ?? throw new NotFoundException($"Cliente {id} no encontrado.");

        if (await TieneOrdenesActivasAsync(id))
            throw new BusinessRuleException("No se puede eliminar un cliente con órdenes de servicio activas.");

        uow.Clientes.Remove(cliente);
        await uow.CommitAsync();
    }

    private async Task<bool> TieneOrdenesActivasAsync(int idCliente)
    {
        var historiales = await uow.HistorialPropietarios.FindAsync(h => h.IdCliente == idCliente);
        var idsVehiculos = historiales.Select(h => h.IdVehiculo).ToHashSet();

        var ordenes = await uow.OrdenesServicio.FindAsync(o => idsVehiculos.Contains(o.IdVehiculo));
        return ordenes.Any(o => o.EstaActiva());
    }

    private async Task<int> ObtenerOCrearDominioAsync(string dominio)
    {
        var existente = (await uow.DominiosCorreo.FindAsync(d => d.Dominio == dominio)).FirstOrDefault();
        if (existente is not null)
            return existente.IdDominioCorreo;

        var nuevo = new DominioCorreo { Dominio = dominio };
        await uow.DominiosCorreo.AddAsync(nuevo);
        await uow.CommitAsync();
        return nuevo.IdDominioCorreo;
    }

    private async Task<int> ObtenerOCrearCodigoTelefonoAsync()
    {
        const string codigoDefault = "+57";
        var existente = (await uow.CodigosTelefono.FindAsync(c => c.Codigo == codigoDefault)).FirstOrDefault();
        if (existente is not null)
            return existente.IdCodigoTelefono;

        var nuevo = new CodigoTelefono { Codigo = codigoDefault, Pais = "Colombia" };
        await uow.CodigosTelefono.AddAsync(nuevo);
        await uow.CommitAsync();
        return nuevo.IdCodigoTelefono;
    }

    private static (string usuario, string dominio) ParseCorreo(string correo)
    {
        var partes = correo.Split('@', 2);
        return partes.Length == 2 ? (partes[0], partes[1]) : (correo, "local");
    }
}
