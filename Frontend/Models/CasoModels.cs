namespace Frontend.Models;

public class CasoRecepcionDto
{
    public int IdOrdenServicio { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public string? MotivoIngreso { get; set; }
    public int? IdCliente { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteDocumento { get; set; }
    public string? ClienteCorreo { get; set; }
    public string? ClienteTelefono { get; set; }
    public int? IdVehiculo { get; set; }
    public string? VehiculoPlaca { get; set; }
    public string? VehiculoVin { get; set; }
    public string? VehiculoMarcaModelo { get; set; }
    public int? VehiculoAnio { get; set; }
    public int? VehiculoKilometraje { get; set; }
    public int? IdTipoServicio { get; set; }
    public string? TipoServicio { get; set; }
    public int? IdMecanico { get; set; }
    public string? MecanicoNombre { get; set; }
    public bool ClienteCompleto { get; set; }
    public bool VehiculoCompleto { get; set; }
    public bool ListoParaConfirmar { get; set; }
}

public class ClienteBusquedaDto
{
    public int IdCliente { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CorreoPrincipal { get; set; } = string.Empty;
    public string TelefonoPrincipal { get; set; } = string.Empty;
}

public class AsignarClienteCasoDto
{
    public int? IdCliente { get; set; }
    public CreateClienteCasoDto? DatosNuevoCliente { get; set; }
}

public class CreateClienteCasoDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public int IdTipoDocumento { get; set; } = 1;
}

public class AsignarVehiculoCasoDto
{
    public int? IdVehiculo { get; set; }
    public CreateVehiculoCasoDto? DatosNuevoVehiculo { get; set; }
}

public class CreateVehiculoCasoDto
{
    public int IdModelo { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int Anio { get; set; } = DateTime.Now.Year;
    public int Kilometraje { get; set; }
}

public class ConfirmarCasoDto
{
    public int IdTipoServicio { get; set; }
    public int IdMecanico { get; set; }
    public string? MotivoIngreso { get; set; }
}

public class ModeloVehiculoCatalogoDto
{
    public int IdModelo { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string TipoVehiculo { get; set; } = string.Empty;
}

public class TipoServicioDto
{
    public int IdTipoServicio { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class TipoDocumentoDto
{
    public int IdTipoDocumento { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public class VehiculoBusquedaDto
{
    public int IdVehiculo { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string TipoVehiculo { get; set; } = string.Empty;
    public string VIN { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int Kilometraje { get; set; }
}
