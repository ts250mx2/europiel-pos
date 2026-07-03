namespace EuropielPos.Domain.Sincronizacion;

/// <summary>
/// Payloads de sincronización de paquetes (ventas) hacia el servidor central.
/// Port de <c>paquete_envio</c>, <c>paquete_servicio_envio</c>,
/// <c>paquete_financiamiento_envio</c> y <c>paciente_envio</c>
/// (DataAnnotations/Objetos.vb).
///
/// IMPORTANTE: el orden de declaración se conserva tal cual (incluidos los
/// campos conekta/euroskin ANTES de bloque) porque JavaScriptSerializer
/// serializaba en ese orden y el servidor espera el JSON así.
/// Fechas como texto <c>yyyy-MM-dd HH:mm:ss</c>.
/// </summary>
#pragma warning disable IDE1006 // nombres snake_case requeridos por el contrato JSON
public class PaqueteEnvio
{
    public int es_euroskin { get; set; }

    public bool cobrable_conekta { get; set; }

    public int conekta_promocion_msi { get; set; }

    public bool tarjeta_no_pudo_validar_cauto { get; set; }

    public string? bloque { get; set; }

    public int id_sucursal { get; set; }

    public int no_caja { get; set; }

    public int id_local { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_paquete { get; set; }

    public int id_paciente_local { get; set; }

    public int? id_paciente { get; set; }

    public int? id_paciente_local_2 { get; set; }

    public int? id_paciente_2 { get; set; }

    public string? nombre_paciente_1 { get; set; }

    public string? nombre_paciente_2 { get; set; }

    public string? fecha_compra { get; set; }

    public bool trat_laser { get; set; }

    public bool trat_corporal { get; set; }

    public bool trat_facial { get; set; }

    public string? forma_pago { get; set; }

    public string? tipo_cobranza { get; set; }

    public string? tarjeta_numero { get; set; }

    public string? tarjeta_tipo { get; set; }

    public string? tarjeta_cvv { get; set; }

    public string? tarjeta_fecha_venc { get; set; }

    public decimal costo_total { get; set; }

    public decimal anticipo { get; set; }

    public int? pagos_x_cubrir { get; set; }

    public decimal pago_unitario { get; set; }

    public string? fecha_pago_1 { get; set; }

    public string? fecha_pago_2 { get; set; }

    public string? fecha_pago_3 { get; set; }

    public string? fecha_pago_4 { get; set; }

    public string? fecha_pago_5 { get; set; }

    public string? fecha_pago_6 { get; set; }

    public string? fecha_pago_7 { get; set; }

    public string? fecha_pago_8 { get; set; }

    public string? fecha_pago_9 { get; set; }

    public string? fecha_pago_10 { get; set; }

    public decimal monto_pago_1 { get; set; }

    public decimal monto_pago_2 { get; set; }

    public decimal monto_pago_3 { get; set; }

    public decimal monto_pago_4 { get; set; }

    public decimal monto_pago_5 { get; set; }

    public decimal monto_pago_6 { get; set; }

    public decimal monto_pago_7 { get; set; }

    public decimal monto_pago_8 { get; set; }

    public decimal monto_pago_9 { get; set; }

    public decimal monto_pago_10 { get; set; }

    public int? id_usuario { get; set; }

    public string? fecha_alta { get; set; }

    public bool trat_laser_2 { get; set; }

    public bool trat_corporal_2 { get; set; }

    public bool trat_facial_2 { get; set; }

    public string? fecha_modificacion { get; set; }

    public int? id_usuario_modifica { get; set; }

    public bool pagado_mitad { get; set; }

    public string? pagado_mitad_fecha { get; set; }

    public bool mostrar_en_sucursales { get; set; }

    public string? tipo_cobranza_1 { get; set; }

    public string? tipo_cobranza_2 { get; set; }

    public string? tipo_cobranza_3 { get; set; }

    public string? tipo_cobranza_4 { get; set; }

    public string? tipo_cobranza_5 { get; set; }

    public string? tipo_cobranza_6 { get; set; }

    public string? tipo_cobranza_7 { get; set; }

    public string? tipo_cobranza_8 { get; set; }

    public string? tipo_cobranza_9 { get; set; }

    public string? tipo_cobranza_10 { get; set; }

    public int? id_promocion { get; set; }

    public decimal costo_total_calculado { get; set; }

    public bool cliente_referido { get; set; }

    public int id_cliente_refirio { get; set; }

    public bool tratamiento_gratis_referido { get; set; }

    public int? id_cliente_referido { get; set; }

    public int? id_medio { get; set; }

    public int? id_sucursal_origen { get; set; }

    public string? fecha_cobranza_automatica { get; set; }

    public bool es_reventa { get; set; }

    public bool no_disponible_por_migracion { get; set; }

    public bool borrado_en_migracion { get; set; }

    public bool proviene_de_migracion { get; set; }

    public string? fecha_recuperacion_cobranza { get; set; }

    public string? fecha_cita_ultimatum { get; set; }

    public string? tarjeta_cs { get; set; }

    public string? tarjeta_numero_t2 { get; set; }

    public string? tarjeta_tipo_t2 { get; set; }

    public string? tarjeta_cvv_t2 { get; set; }

    public string? tarjeta_fecha_venc_t2 { get; set; }

    public string? tarjeta_cs_t2 { get; set; }

    public int? tarjeta_primaria { get; set; }

    public bool paquete_completo { get; set; }

    public bool compartido_enfermera { get; set; }

    public int? id_enfermera { get; set; }

    public int? id_paciente_local_3 { get; set; }

    public int? id_paciente_3 { get; set; }

    public string? nombre_paciente_3 { get; set; }

    public bool trat_laser_3 { get; set; }

    public bool trat_corporal_3 { get; set; }

    public bool trat_facial_3 { get; set; }

    public int? id_paciente_local_4 { get; set; }

    public int? id_paciente_4 { get; set; }

    public string? nombre_paciente_4 { get; set; }

    public bool trat_laser_4 { get; set; }

    public bool trat_corporal_4 { get; set; }

    public bool trat_facial_4 { get; set; }

    public int? id_paciente_local_5 { get; set; }

    public int? id_paciente_5 { get; set; }

    public string? nombre_paciente_5 { get; set; }

    public bool trat_laser_5 { get; set; }

    public bool trat_corporal_5 { get; set; }

    public bool trat_facial_5 { get; set; }

    public decimal saldo_vencido { get; set; }

    public decimal saldo_vencido_5_dias { get; set; }

    public decimal saldo_total { get; set; }

    public bool es_negrita { get; set; }

    public string? fecha_actualizacion_saldos { get; set; }

    public string? observaciones { get; set; }

    public string? token_t1 { get; set; }

    public string? token_t2 { get; set; }

    public string? contrato { get; set; }

    public bool? venta_completa_enfermera { get; set; }

    public int? id_usuario_vta_com_enf { get; set; }

    public string? fecha_vta_com_enf { get; set; }

    public string? no_cuenta { get; set; }

    public string? iban { get; set; }

    public string? dni { get; set; }

    public string? iban_erroneo { get; set; }

    public string? firma_contrato { get; set; }

    public string? tipo_cuenta { get; set; }

    public List<PaqueteFinanciamientoEnvio>? paquete_financiamiento { get; set; }

    public List<PaqueteServicioEnvio>? paquete_servicio { get; set; }

    public PacienteEnvio? paciente1 { get; set; }

    public PacienteEnvio? paciente2 { get; set; }

    public PacienteEnvio? paciente3 { get; set; }

    public PacienteEnvio? paciente4 { get; set; }

    public PacienteEnvio? paciente5 { get; set; }
}

public class PaqueteServicioEnvio
{
    public int id_local { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_paquete_servicio { get; set; }

    public int id_local_paquete { get; set; }

    public int? id_paquete { get; set; }

    public int id_servicio { get; set; }

    public int cantidad { get; set; }

    public int id_paciente_local { get; set; }

    public int id_paciente { get; set; }

    public int es_gratis { get; set; }
}

public class PaqueteFinanciamientoEnvio
{
    public int id_local { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_financiemiento { get; set; }

    public int id_paquete_local { get; set; }

    public int? id_paquete { get; set; }

    public int num_pago { get; set; }

    public decimal monto { get; set; }

    public string? fecha { get; set; }

    public string? tipo { get; set; }

    public int? bandera_manual { get; set; }
}

public class PacienteEnvio
{
    public string? bloque { get; set; }

    public int id_local { get; set; }

    public int no_caja { get; set; }

    public string? fecha_modificacion_local { get; set; }

    public string? fecha_interfaz { get; set; }

    public int? id_paciente { get; set; }

    public string? nombre { get; set; }

    public string? ap_paterno { get; set; }

    public string? ap_materno { get; set; }

    public string? telefono_1 { get; set; }

    public string? telefono_2 { get; set; }

    public string? email { get; set; }

    public int? id_sucursal { get; set; }

    public int? id_sucursal_2 { get; set; }

    public string? fecha_alta { get; set; }

    public int? id_usuario_alta { get; set; }

    public string? identidad { get; set; }

    public int id_tipo_identificacion { get; set; }

    public string? identidad2 { get; set; }

    public int id_tipo_identificacion2 { get; set; }

    public string? domicilio { get; set; }

    public string? colonia { get; set; }

    public string? sexo { get; set; }

    public string? fecha_nacimiento { get; set; }

    public int? edad { get; set; }

    public string? estado { get; set; }

    public string? municipio { get; set; }

    public string? num { get; set; }

    public int id_ciudad { get; set; }

    public int id_estado { get; set; }

    public int id_pais { get; set; }

    public string? codigo_postal { get; set; }

    public string? tipo_identificacion_cliente { get; set; }
}
#pragma warning restore IDE1006
