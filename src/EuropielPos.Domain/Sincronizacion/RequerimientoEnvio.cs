namespace EuropielPos.Domain.Sincronizacion;

/// <summary>
/// Payload de sincronización de requerimientos hacia el servidor central.
/// Port de <c>requerimiento_envio</c> (DataAnnotations/Objetos.vb).
///
/// IMPORTANTE: los nombres de propiedad están en snake_case (incluida la
/// inconsistencia histórica <c>fecha_Vencimiento</c>) y en el mismo orden de
/// declaración, porque el servidor espera el JSON exactamente así.
/// Las fechas viajan como texto <c>yyyy-MM-dd HH:mm:ss</c>.
/// </summary>
#pragma warning disable IDE1006 // nombres snake_case requeridos por el contrato JSON
public class RequerimientoEnvio
{
    public string? bloque { get; set; }

    public int id_sucursal { get; set; }

    public int no_caja { get; set; }

    public int id_local { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_requerimiento { get; set; }

    public int? folio { get; set; }

    public string? fecha_Vencimiento { get; set; }

    public string? concepto { get; set; }

    public decimal monto { get; set; }

    public string? dirigido { get; set; }

    public string? fecha_alta { get; set; }

    public int? id_usuario_alta { get; set; }

    public string? bloque_alta { get; set; }

    public string? fecha_termino { get; set; }

    public int? id_usuario_termino { get; set; }

    public string? bloque_termino { get; set; }

    public string? fecha_borro { get; set; }

    public int? id_usuario_borro { get; set; }

    public string? bloque_borro { get; set; }

    public bool es_borrado { get; set; }

    public string? tipo { get; set; }

    public int? id_tipo_falla { get; set; }
}
#pragma warning restore IDE1006
