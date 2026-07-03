namespace EuropielPos.Domain.Sincronizacion;

/// <summary>
/// Payloads de sincronización de pagos de caja hacia el servidor central.
/// Port de <c>pago_caja_envio</c>, <c>pago_caja_forma_envio</c> y
/// <c>pago_caja_detalle_envio</c> (DataAnnotations/Objetos.vb).
/// Nombres snake_case (incluido <c>FOLIO_RECIBO</c> en mayúsculas) y orden
/// de declaración idénticos al contrato del servidor; fechas como texto
/// <c>yyyy-MM-dd HH:mm:ss</c>.
/// </summary>
#pragma warning disable IDE1006 // nombres snake_case requeridos por el contrato JSON
public class PagoCajaEnvio
{
    public string? bloque { get; set; }

    public int id_sucursal { get; set; }

    public int no_caja { get; set; }

    public int id_local { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_pago { get; set; }

    public string? fecha { get; set; }

    public int? id_usuario { get; set; }

    public string? tipo_recibo { get; set; }

    public string? nombre { get; set; }

    public string? domicilio { get; set; }

    public string? rfc { get; set; }

    public decimal? subtotal { get; set; }

    public decimal? iva { get; set; }

    public decimal? total { get; set; }

    public decimal? pago { get; set; }

    public int? FOLIO_RECIBO { get; set; }

    public int? id_paciente_local { get; set; }

    public int? id_paciente { get; set; }

    public string? fecha_alta { get; set; }

    public int? id_banco { get; set; }

    public int? es_anticipo { get; set; }

    public string? folio_facturacion { get; set; }

    public int? es_euroskin { get; set; }
}

public class PagoCajaFormaEnvio
{
    public string? bloque { get; set; }

    public int id_sucursal { get; set; }

    public int no_caja { get; set; }

    public int id_local { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_pago_local { get; set; }

    public int? id_pago { get; set; }

    public decimal? pago { get; set; }

    public decimal? pago_efectivo { get; set; }

    public decimal? pago_tc { get; set; }

    public decimal? pago_td { get; set; }

    public decimal? pago_sin_categoria { get; set; }

    public decimal? pago_ca { get; set; }

    public decimal? pago_transferencia { get; set; }
}

public class PagoCajaDetalleEnvio
{
    public string? bloque { get; set; }

    public int id_sucursal { get; set; }

    public int no_caja { get; set; }

    public int id_local { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_pago_detalle { get; set; }

    public int? id_pago_local { get; set; }

    public int? id_pago { get; set; }

    public int? id_producto { get; set; }

    public decimal? monto { get; set; }

    public int? id_local_paciente { get; set; }

    public int? id_paciente { get; set; }

    public int? id_paquete_local { get; set; }

    public int? id_paquete { get; set; }

    public int? pago_recuperacion { get; set; }

    public int? id_recuperador { get; set; }
}
#pragma warning restore IDE1006
