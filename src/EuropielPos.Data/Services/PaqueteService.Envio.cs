using System.Text.Json;
using EuropielPos.Data.Entities;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Bloque 2 de <c>PaqueteBL.vb</c>: construcción de los payloads de envío
/// (paquete completo con servicios, financiamientos y hasta 5 pacientes).
/// </summary>
public interface IPaqueteEnvioService
{
    Task<List<Paquete>> RecuperaPaquetesAEnviarAsync(CancellationToken ct = default);

    Task<string> RecuperaJsonPacienteAEnviarAsync(int idPacienteLocal, string bloque, int noCaja, CancellationToken ct = default);

    Task<string> RecuperaJsonPaqueteAEnviarAsync(Paquete paquete, string bloque, int noCaja, int idSucursal, CancellationToken ct = default);

    Task<List<PaqueteServicioEnvio>> RecuperaPaqueteServicioEnvioAsync(int idPaqueteLocal, CancellationToken ct = default);

    Task<List<PaqueteFinanciamientoEnvio>> RecuperaPaqueteFinanciamientoEnvioAsync(int idPaqueteLocal, CancellationToken ct = default);

    Task<PacienteEnvio> RecuperaPacienteEnvioAsync(int idPacienteLocal, CancellationToken ct = default);
}

public partial class PaqueteService : IPaqueteEnvioService
{
    /// <summary>
    /// Paquetes pendientes de sincronizar. Regla original: sin fecha de
    /// interfaz y, si el paquete exige enviar sus servicios
    /// (<c>si_enviar_paquete_servicio=1</c>), solo cuando ya existen.
    /// El mapeo manual de ~110 columnas del original lo hace EF Core.
    /// </summary>
    public Task<List<Paquete>> RecuperaPaquetesAEnviarAsync(CancellationToken ct = default) =>
        _db.Paquete.AsNoTracking()
            .Where(p => p.FechaInterfaz == null
                        && (p.SiEnviarPaqueteServicio == 0
                            || (p.SiEnviarPaqueteServicio == 1
                                && _db.PaqueteServicio.Any(s => s.IdLocalPaquete == p.IdLocal))))
            .ToListAsync(ct);

    public async Task<string> RecuperaJsonPacienteAEnviarAsync(int idPacienteLocal, string bloque, int noCaja, CancellationToken ct = default)
    {
        var paciente = await RecuperaPacienteEnvioAsync(idPacienteLocal, ct);
        paciente.bloque = bloque;
        paciente.no_caja = noCaja;

        return JsonSerializer.Serialize(paciente);
    }

    /// <summary>
    /// JSON completo del paquete a sincronizar, con servicios, financiamientos
    /// y los pacientes 1-5 anidados, replicando el armado del BL original.
    /// </summary>
    public async Task<string> RecuperaJsonPaqueteAEnviarAsync(Paquete paquete, string bloque, int noCaja, int idSucursal, CancellationToken ct = default)
    {
        int idPacienteLocal2 = paquete.IdPacienteLocal2 ?? 0;
        int idPacienteLocal3 = paquete.IdPacienteLocal3 ?? 0;
        int idPacienteLocal4 = paquete.IdPacienteLocal4 ?? 0;
        int idPacienteLocal5 = paquete.IdPacienteLocal5 ?? 0;

        var p = new PaqueteEnvio
        {
            bloque = bloque,
            id_sucursal = idSucursal,
            no_caja = noCaja,
            id_local = paquete.IdLocal,
            fecha_modificacion_local = Fecha(paquete.FechaModificacionLocal),
            fecha_interfaz = Fecha(paquete.FechaInterfaz),
            id_paquete = paquete.IdPaquete,
            id_paciente_local = paquete.IdPacienteLocal ?? 0,
            id_paciente = paquete.IdPaciente,
            nombre_paciente_1 = paquete.NombrePaciente1 ?? string.Empty,
            id_paciente_local_2 = idPacienteLocal2,
            id_paciente_2 = paquete.IdPaciente2,
            nombre_paciente_2 = paquete.NombrePaciente2 ?? string.Empty,
            fecha_compra = Fecha(paquete.FechaCompra),
            trat_laser = paquete.TratLaser ?? false,
            trat_corporal = paquete.TratCorporal ?? false,
            trat_facial = paquete.TratFacial ?? false,
            forma_pago = paquete.FormaPago ?? string.Empty,
            tipo_cobranza = paquete.TipoCobranza ?? string.Empty,
            tarjeta_numero = paquete.TarjetaNumero ?? string.Empty,
            tarjeta_tipo = paquete.TarjetaTipo ?? string.Empty,
            tarjeta_cvv = paquete.TarjetaCvv ?? string.Empty,
            tarjeta_fecha_venc = paquete.TarjetaFechaVenc ?? string.Empty,
            costo_total = paquete.CostoTotal ?? 0m,
            anticipo = paquete.Anticipo ?? 0m,
            pagos_x_cubrir = paquete.PagosXCubrir,
            pago_unitario = paquete.PagoUnitario ?? 0m,
            fecha_pago_1 = Fecha(paquete.FechaPago1),
            fecha_pago_2 = Fecha(paquete.FechaPago2),
            fecha_pago_3 = Fecha(paquete.FechaPago3),
            fecha_pago_4 = Fecha(paquete.FechaPago4),
            fecha_pago_5 = Fecha(paquete.FechaPago5),
            fecha_pago_6 = Fecha(paquete.FechaPago6),
            fecha_pago_7 = Fecha(paquete.FechaPago7),
            fecha_pago_8 = Fecha(paquete.FechaPago8),
            fecha_pago_9 = Fecha(paquete.FechaPago9),
            fecha_pago_10 = Fecha(paquete.FechaPago10),
            monto_pago_1 = paquete.MontoPago1 ?? 0m,
            monto_pago_2 = paquete.MontoPago2 ?? 0m,
            monto_pago_3 = paquete.MontoPago3 ?? 0m,
            monto_pago_4 = paquete.MontoPago4 ?? 0m,
            monto_pago_5 = paquete.MontoPago5 ?? 0m,
            monto_pago_6 = paquete.MontoPago6 ?? 0m,
            monto_pago_7 = paquete.MontoPago7 ?? 0m,
            monto_pago_8 = paquete.MontoPago8 ?? 0m,
            monto_pago_9 = paquete.MontoPago9 ?? 0m,
            monto_pago_10 = paquete.MontoPago10 ?? 0m,
            id_usuario = paquete.IdUsuario,
            fecha_alta = Fecha(paquete.FechaAlta),
            trat_laser_2 = paquete.TratLaser2 ?? false,
            trat_corporal_2 = paquete.TratCorporal2 ?? false,
            trat_facial_2 = paquete.TratFacial2 ?? false,
            fecha_modificacion = Fecha(paquete.FechaModificacion),
            id_usuario_modifica = paquete.IdUsuarioModifica,
            pagado_mitad = paquete.PagadoMitad ?? false,
            pagado_mitad_fecha = Fecha(paquete.PagadoMitadFecha),
            mostrar_en_sucursales = paquete.MostrarEnSucursales ?? false,
            tipo_cobranza_1 = paquete.TipoCobranza1 ?? string.Empty,
            tipo_cobranza_2 = paquete.TipoCobranza2 ?? string.Empty,
            tipo_cobranza_3 = paquete.TipoCobranza3 ?? string.Empty,
            tipo_cobranza_4 = paquete.TipoCobranza4 ?? string.Empty,
            tipo_cobranza_5 = paquete.TipoCobranza5 ?? string.Empty,
            tipo_cobranza_6 = paquete.TipoCobranza6 ?? string.Empty,
            tipo_cobranza_7 = paquete.TipoCobranza7 ?? string.Empty,
            tipo_cobranza_8 = paquete.TipoCobranza8 ?? string.Empty,
            tipo_cobranza_9 = paquete.TipoCobranza9 ?? string.Empty,
            tipo_cobranza_10 = paquete.TipoCobranza10 ?? string.Empty,
            id_promocion = paquete.IdPromocion,
            costo_total_calculado = paquete.CostoTotalCalculado ?? 0m,
            cliente_referido = paquete.ClienteReferido ?? false,
            id_cliente_refirio = paquete.IdClienteRefirio ?? 0,
            tratamiento_gratis_referido = paquete.TratamientoGratisReferido ?? false,
            id_cliente_referido = paquete.IdClienteReferido,
            id_medio = paquete.IdMedio,
            id_sucursal_origen = paquete.IdSucursalOrigen,
            fecha_cobranza_automatica = Fecha(paquete.FechaCobranzaAutomatica),
            es_reventa = paquete.EsReventa ?? false,
            no_disponible_por_migracion = paquete.NoDisponiblePorMigracion ?? false,
            borrado_en_migracion = paquete.BorradoEnMigracion ?? false,
            proviene_de_migracion = paquete.ProvieneDeMigracion ?? false,
            fecha_recuperacion_cobranza = Fecha(paquete.FechaRecuperacionCobranza),
            fecha_cita_ultimatum = Fecha(paquete.FechaCitaUltimatum),
            tarjeta_cs = paquete.TarjetaCs ?? string.Empty,
            tarjeta_numero_t2 = paquete.TarjetaNumeroT2 ?? string.Empty,
            tarjeta_tipo_t2 = paquete.TarjetaTipoT2 ?? string.Empty,
            tarjeta_cvv_t2 = paquete.TarjetaCvvT2 ?? string.Empty,
            tarjeta_fecha_venc_t2 = paquete.TarjetaFechaVencT2 ?? string.Empty,
            tarjeta_cs_t2 = paquete.TarjetaCsT2 ?? string.Empty,
            tarjeta_primaria = paquete.TarjetaPrimaria,
            paquete_completo = paquete.PaqueteCompleto ?? false,
            compartido_enfermera = paquete.CompartidoEnfermera ?? false,
            id_enfermera = paquete.IdEnfermera,
            id_paciente_local_3 = idPacienteLocal3,
            id_paciente_3 = paquete.IdPaciente3,
            nombre_paciente_3 = paquete.NombrePaciente3 ?? string.Empty,
            trat_laser_3 = paquete.TratLaser3 ?? false,
            trat_corporal_3 = paquete.TratCorporal3 ?? false,
            trat_facial_3 = paquete.TratFacial3 ?? false,
            saldo_vencido = paquete.SaldoVencido ?? 0m,
            saldo_vencido_5_dias = paquete.SaldoVencido5Dias ?? 0m,
            saldo_total = paquete.SaldoTotal ?? 0m,
            es_negrita = paquete.EsNegrita ?? false,
            fecha_actualizacion_saldos = Fecha(paquete.FechaActualizacionSaldos),
            observaciones = paquete.Observaciones ?? string.Empty,
            token_t1 = paquete.TokenT1 ?? string.Empty,
            id_paciente_local_4 = idPacienteLocal4,
            id_paciente_4 = paquete.IdPaciente4,
            nombre_paciente_4 = paquete.NombrePaciente4 ?? string.Empty,
            trat_laser_4 = paquete.TratLaser4 ?? false,
            trat_corporal_4 = paquete.TratCorporal4 ?? false,
            trat_facial_4 = paquete.TratFacial4 ?? false,
            id_paciente_local_5 = idPacienteLocal5,
            id_paciente_5 = paquete.IdPaciente5,
            nombre_paciente_5 = paquete.NombrePaciente5 ?? string.Empty,
            trat_laser_5 = paquete.TratLaser5 ?? false,
            trat_corporal_5 = paquete.TratCorporal5 ?? false,
            trat_facial_5 = paquete.TratFacial5 ?? false,
            // Estos cinco conservan null cuando la columna viene null (el
            // original solo los asignaba si tenían valor):
            no_cuenta = paquete.NoCuenta,
            iban = paquete.Iban,
            dni = paquete.Dni,
            firma_contrato = paquete.FirmaContrato,
            tipo_cuenta = paquete.TipoCuenta,
            tarjeta_no_pudo_validar_cauto = paquete.TarjetaNoPudoValidarCauto ?? false,
            cobrable_conekta = paquete.CobrableConekta ?? false,
            conekta_promocion_msi = paquete.ConektaPromocionMsi ?? 0,
            token_t2 = paquete.TokenT2 ?? string.Empty,
            contrato = paquete.Contrato ?? string.Empty,
            venta_completa_enfermera = paquete.VentaCompletaEnfermera,
            id_usuario_vta_com_enf = paquete.IdUsuarioVtaComEnf,
            fecha_vta_com_enf = Fecha(paquete.FechaVtaComEnf),
            es_euroskin = paquete.EsEuroskin ?? 0,
        };

        if (paquete.SiEnviarPaqueteServicio == 1)
            p.paquete_servicio = await RecuperaPaqueteServicioEnvioAsync(paquete.IdLocal, ct);

        p.paquete_financiamiento = await RecuperaPaqueteFinanciamientoEnvioAsync(paquete.IdLocal, ct);

        if ((paquete.IdPacienteLocal ?? 0) > 0)
            p.paciente1 = await RecuperaPacienteEnvioAsync(paquete.IdPacienteLocal!.Value, ct);

        if (idPacienteLocal2 > 0)
            p.paciente2 = await RecuperaPacienteEnvioAsync(idPacienteLocal2, ct);

        if (idPacienteLocal3 > 0)
            p.paciente3 = await RecuperaPacienteEnvioAsync(idPacienteLocal3, ct);

        if (idPacienteLocal4 > 0)
            p.paciente4 = await RecuperaPacienteEnvioAsync(idPacienteLocal4, ct);

        if (idPacienteLocal5 > 0)
            p.paciente5 = await RecuperaPacienteEnvioAsync(idPacienteLocal5, ct);

        return JsonSerializer.Serialize(p);
    }

    public async Task<List<PaqueteServicioEnvio>> RecuperaPaqueteServicioEnvioAsync(int idPaqueteLocal, CancellationToken ct = default)
    {
        var filas = await _db.PaqueteServicio.AsNoTracking()
            .Where(s => s.IdLocalPaquete == idPaqueteLocal)
            .ToListAsync(ct);

        return filas.Select(s => new PaqueteServicioEnvio
        {
            id_local = s.IdLocal,
            fecha_modificacion_local = Fecha(s.FechaModificacionLocal),
            fecha_interfaz = Fecha(s.FechaInterfaz),
            id_paquete_servicio = s.IdPaqueteServicio,
            id_local_paquete = s.IdLocalPaquete ?? 0,
            id_paquete = s.IdPaquete,
            id_servicio = s.IdServicio ?? 0,
            cantidad = s.Cantidad ?? 0,
            id_paciente_local = s.IdPacienteLocal ?? 0,
            id_paciente = s.IdPaciente ?? 0,
            es_gratis = s.EsGratis ?? 0,
        }).ToList();
    }

    public async Task<List<PaqueteFinanciamientoEnvio>> RecuperaPaqueteFinanciamientoEnvioAsync(int idPaqueteLocal, CancellationToken ct = default)
    {
        var filas = await _db.PaqueteFinanciamiento.AsNoTracking()
            .Where(f => f.IdPaqueteLocal == idPaqueteLocal)
            .ToListAsync(ct);

        return filas.Select(f => new PaqueteFinanciamientoEnvio
        {
            id_local = f.IdLocal,
            fecha_modificacion_local = Fecha(f.FechaModificacionLocal),
            fecha_interfaz = Fecha(f.FechaInterfaz),
            id_financiemiento = f.IdFinanciemiento,
            id_paquete_local = f.IdPaqueteLocal ?? 0,
            id_paquete = f.IdPaquete,
            num_pago = f.NumPago ?? 0,
            monto = f.Monto ?? 0m,
            fecha = Fecha(f.Fecha),
            tipo = f.Tipo ?? string.Empty,
            bandera_manual = f.BanderaManual,
        }).ToList();
    }

    /// <remarks>Si el paciente no existe devuelve un DTO vacío, como el original.
    /// Se replica también la peculiaridad de que <c>edad</c> viaja como bandera
    /// 1/0 (tiene o no edad capturada) y <c>id_sucursal_2</c> repite id_sucursal.</remarks>
    public async Task<PacienteEnvio> RecuperaPacienteEnvioAsync(int idPacienteLocal, CancellationToken ct = default)
    {
        var pa = await _db.Paciente.AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdLocal == idPacienteLocal, ct);

        if (pa is null)
            return new PacienteEnvio();

        return new PacienteEnvio
        {
            id_local = pa.IdLocal,
            fecha_modificacion_local = Fecha(pa.FechaModificacionLocal),
            fecha_interfaz = Fecha(pa.FechaInterfaz),
            id_paciente = pa.IdPaciente,
            nombre = pa.Nombre ?? string.Empty,
            ap_paterno = pa.ApPaterno ?? string.Empty,
            ap_materno = pa.ApMaterno ?? string.Empty,
            telefono_1 = pa.Telefono1 ?? string.Empty,
            telefono_2 = pa.Telefono2 ?? string.Empty,
            email = pa.Email ?? string.Empty,
            id_sucursal = pa.IdSucursal ?? 0,
            id_sucursal_2 = pa.IdSucursal ?? 0, // el original repetía id_sucursal
            fecha_alta = Fecha(pa.FechaAlta),
            id_usuario_alta = pa.IdUsuarioAlta ?? 0,
            identidad = pa.Identidad ?? string.Empty,
            id_tipo_identificacion = pa.IdTipoIdentificacion ?? 0,
            identidad2 = pa.Identidad2 ?? string.Empty,
            id_tipo_identificacion2 = pa.IdTipoIdentificacion2 ?? 0,
            domicilio = pa.Domicilio ?? string.Empty,
            colonia = pa.Colonia ?? string.Empty,
            sexo = pa.Sexo ?? string.Empty,
            fecha_nacimiento = Fecha(pa.FechaNacimiento),
            edad = pa.Edad != null ? 1 : 0, // bandera 1/0 del original
            estado = pa.Estado ?? string.Empty,
            municipio = pa.Municipio ?? string.Empty,
            num = pa.Num ?? string.Empty,
            tipo_identificacion_cliente = pa.TipoIdentificacionCliente ?? string.Empty,
            id_ciudad = pa.IdCiudad ?? 0,
            id_estado = pa.IdEstado ?? 0,
            id_pais = pa.IdPais ?? 0,
            codigo_postal = pa.CodigoPostal ?? string.Empty,
        };
    }
}
