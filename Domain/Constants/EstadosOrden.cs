namespace Domain.Constants;

public static class EstadosOrden
{
    public const string EnRegistro = "En registro";
    public const string Pendiente = "Pendiente";
    public const string EnProceso = "En Proceso";
    public const string Completada = "Completada";
    public const string Cancelada = "Cancelada";

    // Flujo extendido (nuevo)
    public const string Recibido = "Recibido";
    public const string DiagnosticoEnProceso = "Diagnóstico en proceso";
    public const string PendienteAprobacionJefe = "Pendiente aprobación jefe";
    public const string PendienteAprobacionCliente = "Pendiente aprobación cliente";
    public const string RechazadoCliente = "Rechazado por cliente";
    public const string AprobadoParcial = "Aprobado parcial";
    public const string AprobadoTotal = "Aprobado total";
    public const string ReparacionEnProceso = "Reparación en proceso";
    public const string ListoParaEntrega = "Listo para entrega";
    public const string PendientePago = "Pendiente de pago";
    public const string Pagado = "Pagado";
    public const string Entregado = "Entregado";
    public const string Cerrado = "Cerrado";
}
