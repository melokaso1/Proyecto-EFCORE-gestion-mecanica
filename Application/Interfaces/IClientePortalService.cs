using Application.Dtos;

namespace Application.Interfaces;

public interface IClientePortalService
{
    Task<PagedResultDto<ClientePortalOrdenDto>> ListarMisOrdenesAsync(
        int idUsuario,
        int page,
        int size,
        string? estado);

    Task DecidirCostoAsync(int idUsuario, int idOrdenServicio, ClienteDecisionCostoDto dto);

    Task<IReadOnlyList<ReparacionItemDto>> ListarReparacionesOrdenAsync(int idUsuario, int idOrdenServicio);
}

