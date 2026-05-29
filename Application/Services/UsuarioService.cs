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
    public async Task<UsuarioDto> CrearAsync(CreateUsuarioDto dto) =>
        await CrearUsuarioInternoAsync(dto);

    public async Task<UsuarioDto> RegistrarAdminAsync(CreateUsuarioDto dto, bool invocadoPorAdmin)
    {
        if (!invocadoPorAdmin && await uow.Usuarios.ExisteConRolAsync("Admin"))
            throw new BusinessRuleException(
                "Ya existe un administrador en el sistema. Solicita acceso al admin del taller o inicia sesión como administrador para registrar otro.");

        dto.IdRol = 1;
        return await CrearUsuarioInternoAsync(dto);
    }

    public async Task<UsuarioDto> RegistrarUsuarioAsync(RegistroUsuarioDto dto) =>
        await CrearUsuarioInternoAsync(new CreateUsuarioDto
        {
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Correo = dto.Correo,
            Password = dto.Password
        }, asignarRol: false);

    public Task<bool> ExisteAdminAsync() =>
        uow.Usuarios.ExisteConRolAsync("Admin");

    private async Task<UsuarioDto> CrearUsuarioInternoAsync(CreateUsuarioDto dto, bool asignarRol = true)
    {
        if (await BuscarUsuarioPorCorreoAsync(dto.Correo) is not null)
            throw new ConflictException("Ya existe un usuario registrado con ese correo.");

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

        List<Rol> roles = [];
        if (asignarRol)
        {
            if (dto.IdRol <= 0)
                throw new BusinessRuleException("Debe especificarse un rol.");

            var rol = await uow.Roles.GetByIdAsync(dto.IdRol)
                ?? throw new NotFoundException($"Rol {dto.IdRol} no encontrado.");
            roles.Add(rol);
        }

        var usuario = new Usuario
        {
            IdPersona = persona.IdPersona,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Estado = true,
            Persona = persona,
            Roles = roles
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

        if (usuario.Roles.Count == 0)
            throw new BusinessRuleException(
                "Tu cuenta está pendiente de aprobación. El administrador debe asignarte un rol antes de ingresar.");

        return tokenGenerator.Generate(usuario, dto.Correo);
    }

    public async Task<PagedResultDto<UsuarioDto>> ListarAsync(int page, int size)
    {
        var (items, total) = await uow.Usuarios.GetPagedAsync(page, size, u => u.Estado);
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

    public async Task<PagedResultDto<UsuarioDto>> ListarEmpleadosAsync(int page, int size)
    {
        var (items, total) = await uow.Usuarios.GetEmpleadosPagedAsync(page, size);
        var dtos = items.Select(u => mapper.Map<UsuarioDto>(u)).ToList();

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

        if (usuario.Roles.Any(r => r.NombreRol == "Admin"))
            throw new BusinessRuleException("No puedes modificar el rol de un administrador.");

        var rol = await uow.Roles.GetByIdAsync(idRol)
            ?? throw new NotFoundException($"Rol {idRol} no encontrado.");

        if (rol.NombreRol is not ("Mecánico" or "Recepcionista" or "Cliente" or "JefeMecanicos" or "JefeBodega"))
            throw new BusinessRuleException("No se puede asignar ese rol desde este módulo.");

        usuario.Roles.Clear();
        usuario.Roles.Add(rol);

        uow.Usuarios.Update(usuario);
        await uow.CommitAsync();
    }

    public async Task AsignarEspecializacionesAsync(int idUsuario, AsignarEspecializacionesDto dto)
    {
        var usuario = await uow.Usuarios.GetByIdAsync(idUsuario)
            ?? throw new NotFoundException($"Usuario {idUsuario} no encontrado.");

        if (!usuario.Roles.Any(r => r.NombreRol == "Mecánico"))
            throw new BusinessRuleException("Solo se pueden asignar especializaciones a mecánicos.");

        var ids = dto.IdsEspecializaciones.Distinct().ToList();
        if (ids.Count == 0)
            throw new BusinessRuleException("Debe seleccionar al menos una especialización.");

        var especializaciones = (await uow.EspecializacionesMecanico.FindAsync(e => e.Activo)).ToList();
        var invalidas = ids.Except(especializaciones.Select(e => e.IdEspecializacionMecanico)).ToList();
        if (invalidas.Count > 0)
            throw new BusinessRuleException("Una o más especializaciones no son válidas.");

        usuario.Especializaciones.Clear();
        foreach (var id in ids)
        {
            var esp = especializaciones.First(e => e.IdEspecializacionMecanico == id);
            usuario.Especializaciones.Add(esp);
        }

        uow.Usuarios.Update(usuario);
        await uow.CommitAsync();
    }

    public async Task EliminarAsync(int id, int idUsuarioInvocador)
    {
        if (id == idUsuarioInvocador)
            throw new BusinessRuleException("No puedes desactivar tu propia cuenta.");

        var usuario = await uow.Usuarios.GetByIdAsync(id)
            ?? throw new NotFoundException($"Usuario {id} no encontrado.");

        if (!usuario.Estado)
            throw new NotFoundException($"Usuario {id} no encontrado.");

        if (usuario.Roles.Any(r => r.NombreRol == "Admin"))
            throw new BusinessRuleException("No se puede desactivar un administrador.");

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
        var completo = await uow.Usuarios.GetByIdAsync(usuario.IdUsuario);
        if (completo is null)
            return;

        usuario.Persona = completo.Persona;
        usuario.Roles = completo.Roles;
        usuario.Especializaciones = completo.Especializaciones;

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
