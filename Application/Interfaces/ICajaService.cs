using Application.Dtos;

namespace Application.Interfaces;

public interface ICajaService
{
    Task<PagoYFacturaDto> RegistrarPagoAsync(int idUsuarioCaja, RegistrarPagoDto dto);
    Task<bool> OrdenEstaPagadaAsync(int idOrdenServicio);
    Task<IReadOnlyList<OrdenPendientePagoDto>> ListarOrdenesPendientesPagoAsync();
    Task<IReadOnlyList<PagoDto>> ListarPagosRecientesAsync(int limit = 20);
}

