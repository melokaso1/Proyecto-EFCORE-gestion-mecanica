namespace Api.Models;

public record AjustarStockRequest(int Cantidad);

public record GenerarFacturaRequest(int IdOrdenServicio, decimal ManoObra);

public record AsignarRolRequest(int IdRol);
