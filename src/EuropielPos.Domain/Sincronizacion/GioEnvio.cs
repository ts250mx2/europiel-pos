namespace EuropielPos.Domain.Sincronizacion;

/// <summary>
/// Payload de sincronización de GIO (personal de intendencia/operación).
/// Port de <c>gio_envio</c> (DataAnnotations/Objetos.vb). Nombres snake_case
/// y orden de declaración idénticos al contrato que espera el servidor;
/// fechas como texto <c>yyyy-MM-dd HH:mm:ss</c>.
/// </summary>
#pragma warning disable IDE1006 // nombres snake_case requeridos por el contrato JSON
public class GioEnvio
{
    public string? bloque { get; set; }

    public int id_sucursal { get; set; }

    public int no_caja { get; set; }

    public int id_local { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_gio { get; set; }

    public string? nombre { get; set; }

    public string? nombre_cuenta { get; set; }

    public string? clave_interbancaria { get; set; }

    public string? banco { get; set; }

    public string? id_estado { get; set; }

    public string? id_ciudad { get; set; }

    public string? telefono { get; set; }

    public string? horario { get; set; }

    public int? id_turno { get; set; }

    public bool es_activo { get; set; }

    public bool es_cambio_sucursal { get; set; }

    public string? fecha_registro { get; set; }
}

/// <summary>
/// Payload de sincronización de asistencias GIO.
/// Port de <c>gio_asistencia_envio</c> (DataAnnotations/Objetos.vb).
/// </summary>
public class GioAsistenciaEnvio
{
    public string? bloque { get; set; }

    public int id_sucursal { get; set; }

    public int no_caja { get; set; }

    public int id_local { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_detalle { get; set; }

    public int id_gio_local { get; set; }

    public int? id_gio { get; set; }

    public string? fecha { get; set; }

    public int id_turno { get; set; }

    public string? horario { get; set; }

    public string? hora_inicio { get; set; }

    public string? hora_fin { get; set; }

    public int? id_usuario_registro { get; set; }

    public string? fecha_registro { get; set; }
}
#pragma warning restore IDE1006
