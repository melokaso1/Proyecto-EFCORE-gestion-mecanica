using Frontend.Models;

namespace Frontend.Services;

public interface IClientePortalService
{
    Task<PagedClientePortalOrdenesResult> GetMisOrdenesAsync(int page = 1, int size = 20, string? estado = null);
    Task<List<ReparacionItemDto>> GetReparacionesOrdenAsync(int idOrdenServicio);
    Task DecidirCostoAsync(int idOrdenServicio, bool aprobado, string? comentario);
}

