using Application.Dtos;

namespace Application.Interfaces;

public interface ICajaService
{
    Task<PagoYFacturaDto> RegistrarPagoAsync(int idUsuarioCaja, RegistrarPagoDto dto);
    Task<bool> OrdenEstaPagadaAsync(int idOrdenServicio);
}

