using Application.Dtos;
using Application.Interfaces;

namespace Application.Services;

public class DashboardService(IUnitOfWork uow, IOrdenServicioService ordenServicioService)
    : IDashboardService
{
    public async Task<AdminDashboardDto> ObtenerAdminAsync()
    {
        var (_, totalUsuarios) = await uow.Usuarios.GetPagedAsync(1, 1, _ => true);
        var usuarios = await uow.Usuarios.GetAllAsync();
        var pendientes = 0;
        var empleados = 0;
        foreach (var u in usuarios)
        {
            var completo = await uow.Usuarios.GetByIdAsync(u.IdUsuario);
            if (completo is null)
                continue;

            if (completo.Roles.Count == 0)
                pendientes++;
            else if (completo.Roles.Any(r => r.NombreRol is "Mecánico" or "Recepcionista"))
                empleados++;
        }

        var (_, totalClientes) = await uow.Clientes.GetPagedAsync(1, 1, _ => true);
        var (_, totalVehiculos) = await uow.Vehiculos.GetPagedAsync(1, 1, _ => true);

        var estados = await uow.EstadosOrden.GetAllAsync();
        var ordenesPorEstado = new Dictionary<string, int>();
        foreach (var estado in estados)
        {
            var (_, count) = await uow.OrdenesServicio.GetPagedAsync(1, 1,
                o => o.IdEstadoOrden == estado.IdEstadoOrden);
            ordenesPorEstado[estado.Nombre] = count;
        }

        return new AdminDashboardDto
        {
            TotalUsuarios = totalUsuarios,
            UsuariosPendientesRol = pendientes,
            TotalClientes = totalClientes,
            TotalVehiculos = totalVehiculos,
            TotalEmpleados = empleados,
            OrdenesPorEstado = ordenesPorEstado
        };
    }

    public async Task<RecepcionDashboardDto> ObtenerRecepcionAsync()
    {
        var (_, totalClientes) = await uow.Clientes.GetPagedAsync(1, 1, _ => true);
        var (_, totalVehiculos) = await uow.Vehiculos.GetPagedAsync(1, 1, _ => true);

        var hoy = DateTime.UtcNow.Date;
        var manana = hoy.AddDays(1);
        var (_, ordenesHoy) = await uow.OrdenesServicio.GetPagedAsync(1, 1,
            o => o.FechaIngreso >= hoy && o.FechaIngreso < manana);

        var recientes = await ordenServicioService.ListarAsync(1, 8, null, null, null, null, null);

        return new RecepcionDashboardDto
        {
            TotalClientes = totalClientes,
            TotalVehiculos = totalVehiculos,
            OrdenesHoy = ordenesHoy,
            OrdenesRecientes = recientes.Items
        };
    }
}
