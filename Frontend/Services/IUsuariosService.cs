using Frontend.Models;

namespace Frontend.Services;

public interface IUsuariosService
{
    Task<bool> IsAdminRegistrationAvailableAsync();
    Task<(bool Success, string? Error)> RegisterAdminAsync(RegisterUserDto dto);
    Task<(bool Success, string? Error)> RegisterUsuarioAsync(RegisterUserDto dto);
    Task<PagedUsuariosResult> GetUsuariosAsync(int page, int size);
    Task<(bool Success, string? Error)> AsignarRolAsync(int idUsuario, int idRol);
    Task<(bool Success, string? Error)> AsignarEspecializacionesAsync(int idUsuario, IReadOnlyList<int> idsEspecializaciones);
    Task<List<EspecializacionMecanicoDto>> GetEspecializacionesAsync();
    Task<(bool Success, string? Error)> EliminarUsuarioAsync(int idUsuario);
}
