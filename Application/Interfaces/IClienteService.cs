using Application.Dtos;

namespace Application.Interfaces;

public interface IClienteService
{
    Task<ClienteDto> RegistrarClienteConVehiculoAsync(CreateClienteDto dto);
    Task<ClienteDto> RegistrarClienteAsync(CreateClienteDto dto);
    Task<ClienteDto?> BuscarPorDocumentoAsync(string numeroDocumento);
    Task<PagedResultDto<ClienteDto>> ListarClientesAsync(int page, int size, string? filtro);
    Task<ClienteDto?> ObtenerPorIdAsync(int id);
    Task ActualizarAsync(int id, CreateClienteDto dto);
    Task EliminarAsync(int id);
}
