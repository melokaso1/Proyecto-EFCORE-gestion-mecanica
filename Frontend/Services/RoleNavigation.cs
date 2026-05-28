using System.Security.Claims;

namespace Frontend.Services;

public static class RoleNavigation
{
    public static string GetHomeRoute(ClaimsPrincipal user)
    {
        if (user.IsInRole("Admin"))
            return "/admin";
        if (user.IsInRole("JefeMecanicos"))
            return "/jefe-mecanicos";
        if (user.IsInRole("Mecánico"))
            return "/mecanico";
        if (user.IsInRole("Recepcionista"))
            return "/recepcion";
        if (user.IsInRole("Cliente"))
            return "/cliente";
        return "/";
    }
}
