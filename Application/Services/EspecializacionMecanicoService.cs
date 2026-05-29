using Application.Dtos;
using Application.Interfaces;
using MapsterMapper;

namespace Application.Services;

public class EspecializacionMecanicoService(IUnitOfWork uow, IMapper mapper) : IEspecializacionMecanicoService
{
    public async Task<IReadOnlyList<EspecializacionMecanicoDto>> ListarActivasAsync()
    {
        var items = await uow.EspecializacionesMecanico.FindAsync(e => e.Activo);
        return items
            .OrderBy(e => e.Nombre)
            .Select(mapper.Map<EspecializacionMecanicoDto>)
            .ToList();
    }
}
