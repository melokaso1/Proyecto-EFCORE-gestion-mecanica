using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services;

public class UsuarioService(
    IUnitOfWork uow,
    IMapper mapper,
    ITokenGenerator tokenGenerator) : IUsuarioService
{
    public async Task<UsuarioDto> CrearAsync(CreateUsuarioDto dto)
    {
        var persona = new Persona
        {
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            FechaRegistro = DateTime.UtcNow
        };
        await uow.Personas.AddAsync(persona);
        await uow.CommitAsync();

        var (usuarioCorreo, dominio) = ParseCorreo(dto.Correo);
        var dominioCorreo = (await uow.DominiosCorreo.FindAsync(d => d.Dominio == dominio)).FirstOrDefault()
            ?? await CrearDominioAsync(dominio);

        await uow.CorreosPersona.AddAsync(new CorreoPersona
        {
            IdPersona = persona.IdPersona,
            IdDominioCorreo = dominioCorreo.IdDominioCorreo,
            UsuarioCorreo = usuarioCorreo,
            EsPrincipal = true
        });

        var rol = await uow.Roles.GetByIdAsync(dto.IdRol)
            ?? throw new NotFoundException($"Rol {dto.IdRol} no encontrado.");

        var usuario = new Usuario
        {
            IdPersona = persona.IdPersona,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Estado = true,
            Persona = persona,
            Roles = [rol]
        };

        await uow.Usuarios.AddAsync(usuario);
        await uow.CommitAsync();

        return mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await BuscarUsuarioPorCorreoAsync(dto.Correo);
        if (usuario is null || !usuario.Estado)
            throw new NotFoundException("Credenciales inválidas.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new NotFoundException("Credenciales inválidas.");

        await CargarUsuarioAsync(usuario);
        return tokenGenerator.Generate(usuario, dto.Correo);
    }

    public async Task<PagedResultDto<UsuarioDto>> ListarAsync(int page, int size)
    {
        var (items, total) = await uow.Usuarios.GetPagedAsync(page, size, _ => true);
        var dtos = new List<UsuarioDto>();
        foreach (var usuario in items)
        {
            await CargarUsuarioAsync(usuario);
            dtos.Add(mapper.Map<UsuarioDto>(usuario));
        }

        return new PagedResultDto<UsuarioDto>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = page,
            PageSize = size
        };
    }

    public async Task AsignarRolAsync(int idUsuario, int idRol)
    {
        var usuario = await uow.Usuarios.GetByIdAsync(idUsuario)
            ?? throw new NotFoundException($"Usuario {idUsuario} no encontrado.");

        var rol = await uow.Roles.GetByIdAsync(idRol)
            ?? throw new NotFoundException($"Rol {idRol} no encontrado.");

        if (usuario.Roles.All(r => r.IdRol != idRol))
            usuario.Roles.Add(rol);

        uow.Usuarios.Update(usuario);
        await uow.CommitAsync();
    }

    public async Task DesactivarAsync(int id)
    {
        var usuario = await uow.Usuarios.GetByIdAsync(id)
            ?? throw new NotFoundException($"Usuario {id} no encontrado.");

        usuario.Estado = false;
        uow.Usuarios.Update(usuario);
        await uow.CommitAsync();
    }

    private async Task<Usuario?> BuscarUsuarioPorCorreoAsync(string correo)
    {
        var (usuarioCorreo, dominio) = ParseCorreo(correo);
        var dominioEntidad = (await uow.DominiosCorreo.FindAsync(d => d.Dominio == dominio)).FirstOrDefault();
        if (dominioEntidad is null)
            return null;

        var correos = await uow.CorreosPersona.FindAsync(c =>
            c.UsuarioCorreo == usuarioCorreo && c.IdDominioCorreo == dominioEntidad.IdDominioCorreo);

        var correoPersona = correos.FirstOrDefault(c => c.EsPrincipal) ?? correos.FirstOrDefault();
        if (correoPersona is null)
            return null;

        return (await uow.Usuarios.FindAsync(u => u.IdPersona == correoPersona.IdPersona)).FirstOrDefault();
    }

    private async Task CargarUsuarioAsync(Usuario usuario)
    {
        usuario.Persona ??= await uow.Personas.GetByIdAsync(usuario.IdPersona);
        if (usuario.Persona is not null)
        {
            usuario.Persona.CorreosPersona = (await uow.CorreosPersona
                .FindAsync(c => c.IdPersona == usuario.IdPersona)).ToList();
        }
    }

    private async Task<DominioCorreo> CrearDominioAsync(string dominio)
    {
        var entidad = new DominioCorreo { Dominio = dominio };
        await uow.DominiosCorreo.AddAsync(entidad);
        await uow.CommitAsync();
        return entidad;
    }

    private static (string usuario, string dominio) ParseCorreo(string correo)
    {
        var partes = correo.Split('@', 2);
        return partes.Length == 2 ? (partes[0], partes[1]) : (correo, "local");
    }
}
