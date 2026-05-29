using Application.Dtos;

namespace Application.Interfaces;

public interface IDiagnosticoService
{
    Task<DiagnosticoDto?> ObtenerAsync(int idOrdenServicio);
    Task<DiagnosticoDto> UpsertAsync(int idOrdenServicio, int idMecanico, UpsertDiagnosticoDto dto);
    Task IniciarDiagnosticoAsync(int idOrdenServicio, int idMecanico);

    Task<IReadOnlyList<ReparacionItemDto>> ListarReparacionesAsync(int idOrdenServicio);
    Task<ReparacionItemDto> CrearReparacionAsync(int idOrdenServicio, int idMecanico, CreateReparacionItemDto dto);
    Task<ReparacionItemDto> ActualizarReparacionAsync(int idOrdenServicio, int idReparacionItem, int idMecanico, UpdateReparacionItemDto dto);

    Task<ReparacionItemDto> DecidirJefeAsync(int idOrdenServicio, int idReparacionItem, int idJefeUsuario, DecisionJefeDto dto);
    Task<IReadOnlyList<ReparacionItemDto>> DecidirClienteAsync(int idOrdenServicio, int idClienteUsuario, Dictionary<int, DecisionClienteDto> decisiones);

    Task<IReadOnlyList<ReparacionItemDto>> ListarPendientesJefeAsync(int page, int size);

    Task<ReparacionItemDto> IniciarReparacionAsync(int idOrdenServicio, int idReparacionItem, int idMecanico);
    Task<ReparacionItemDto> TerminarReparacionAsync(int idOrdenServicio, int idReparacionItem, int idMecanico);
}

