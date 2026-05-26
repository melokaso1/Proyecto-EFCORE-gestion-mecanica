using Application.Dtos;

namespace Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDto> CrearAsync(CreateUsuarioDto dto);
    Task<TokenResponseDto> LoginAsync(LoginDto dto);
    Task<PagedResultDto<UsuarioDto>> ListarAsync(int page, int size);
    Task AsignarRolAsync(int idUsuario, int idRol);
    Task DesactivarAsync(int id);
}
