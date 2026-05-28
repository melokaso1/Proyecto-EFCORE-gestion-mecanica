using Frontend.Models;

namespace Frontend.Services;

public interface IClientesService
{
    Task<PagedResult<ClienteDto>> GetClientesAsync(int page, int size, string? filtro);
    Task CreateClienteAsync(CreateClienteDto dto);
    Task UpdateClienteAsync(int id, CreateClienteDto dto);
    Task DeleteClienteAsync(int id);
}
