using Application.Dtos;

namespace Application.Interfaces;

public interface IAuditoriaService
{
    Task RegistrarAsync(
        int idUsuario,
        string tipoAccion,
        string entidad,
        int idRegistro,
        string? descripcion);
    Task<PagedResultDto<AuditoriaDto>> ListarAsync(
        int page,
        int size,
        string? entidad,
        int? idUsuario);
}
