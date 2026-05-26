using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class AuditoriaService(IUnitOfWork uow) : IAuditoriaService
{
    public async Task RegistrarAsync(
        int idUsuario,
        string tipoAccion,
        string entidad,
        int idRegistro,
        string? descripcion)
    {
        var tipo = (await uow.TiposAccionAuditoria.FindAsync(t => t.Nombre == tipoAccion)).FirstOrDefault();
        if (tipo is null)
        {
            tipo = new TipoAccionAuditoria { Nombre = tipoAccion };
            await uow.TiposAccionAuditoria.AddAsync(tipo);
            await uow.CommitAsync();
        }

        await uow.Auditorias.AddAsync(new Auditoria
        {
            IdUsuario = idUsuario,
            IdTipoAccionAuditoria = tipo.IdTipoAccionAuditoria,
            EntidadAfectada = entidad,
            IdRegistroAfectado = idRegistro,
            FechaHora = DateTime.UtcNow,
            Descripcion = descripcion
        });

        await uow.CommitAsync();
    }

    public async Task<PagedResultDto<AuditoriaDto>> ListarAsync(
        int page,
        int size,
        string? entidad,
        int? idUsuario)
    {
        var (items, total) = await uow.Auditorias.GetPagedAsync(page, size, a =>
            (entidad == null || a.EntidadAfectada == entidad) &&
            (idUsuario == null || a.IdUsuario == idUsuario));

        var dtos = new List<AuditoriaDto>();
        foreach (var auditoria in items)
        {
            var tipo = await uow.TiposAccionAuditoria.GetByIdAsync(auditoria.IdTipoAccionAuditoria);
            dtos.Add(new AuditoriaDto
            {
                IdAuditoria = auditoria.IdAuditoria,
                IdUsuario = auditoria.IdUsuario,
                TipoAccion = tipo?.Nombre ?? string.Empty,
                EntidadAfectada = auditoria.EntidadAfectada,
                IdRegistroAfectado = auditoria.IdRegistroAfectado,
                FechaHora = auditoria.FechaHora,
                Descripcion = auditoria.Descripcion
            });
        }

        return new PagedResultDto<AuditoriaDto>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = page,
            PageSize = size
        };
    }
}
