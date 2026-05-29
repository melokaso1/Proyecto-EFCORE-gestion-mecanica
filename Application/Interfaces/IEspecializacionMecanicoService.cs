using Application.Dtos;

namespace Application.Interfaces;

public interface IEspecializacionMecanicoService
{
    Task<IReadOnlyList<EspecializacionMecanicoDto>> ListarActivasAsync();
}
