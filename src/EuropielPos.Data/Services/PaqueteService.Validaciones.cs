using System.Data;
using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Bloque 3 de <c>PaqueteBL.vb</c>: validaciones de venta/reventa, tokens de
/// autorización, tarjetas y operaciones sobre el paquete.
/// </summary>
public interface IPaqueteValidacionesService
{
    Task<string> ValidaReventaPacienteConCobranzaRecuperadorAsync(int idPacienteLocal, CancellationToken ct = default);

    Task<string> ValidaReventaPacienteAsync(int idPacienteLocal, decimal montoAnticipo, string formaPago,
        string tipoCobranza, DateTime fechaPago1, int idPaqueteLocal, bool ventaLiquidada, CancellationToken ct = default);

    Task<string> ValidaReventaSaldoPorPacientesAsync(int idPaciente1, int idPaciente2, int idPaciente3,
        int idPaciente4, int idPaciente5, int idSucursal, bool ventaLiquidada, CancellationToken ct = default);

    Task<int> ValidaPacienteCobranzaCteNoLiquidadoAsync(int idPacienteLocal, CancellationToken ct = default);

    Task ActualizaPaqueteServicioAsync(int idPaqueteLocal, bool tratLaser, bool tratLaser2, bool tratLaser3,
        bool tratLaser4, bool tratLaser5, CancellationToken ct = default);

    Task<Paquete?> RecuperaPaquetePorIdAsync(int idPaqueteLocal, CancellationToken ct = default);

    Task<DataTable> RecuperaPromocionServiciosDefaultAsync(int idPaquete, int idPaciente, CancellationToken ct = default);

    Task<DataTable> ValidaAutorizacionVentaAsync(int idPaqueteConsulta, int idPaciente, int idPaciente1, int idPaciente2,
        int idPaciente3, int idPaciente4, int idPaciente5, string pacienteNuevo, string numTarjeta, string numTarjetaT2, CancellationToken ct = default);

    Task<DataTable> ValidaAutorizacionVentaClienteRepetidoAsync(IReadOnlyList<(string Nombre, string ApPaterno, string ApMaterno)> nombres,
        IReadOnlyList<(string Telefono, string Celular)> telefonos, CancellationToken ct = default);

    Task<int> ValidaTokenAutorizacionVentaAsync(int idPaqueteLocal, string token, string tipo, CancellationToken ct = default);

    Task<decimal> RecuperaSaldoTotalPaquetesAsync(int idPacienteLocal1, int idPacienteLocal2, int idPacienteLocal3,
        int idPacienteLocal4, int idPacienteLocal5, CancellationToken ct = default);

    Task<DataTable> ValidarUsarDatosTarjetaAnteriorAsync(int idPacienteLocal, CancellationToken ct = default);

    Task GuardaTokenAutorizacionVentaAsync(int idPaqueteLocal, string token, string tipo, CancellationToken ct = default);

    Task<DataTable> ValidaTarjetaNuevoPaqueteAsync(int idPaqueteLocal, CancellationToken ct = default);

    Task<DataTable> ValidaTarjetaNuevaAsync(string numTarjeta, CancellationToken ct = default);

    Task<DataTable> RecuperaReimpresionContratoAsync(CancellationToken ct = default);

    Task<string> ValidaReventaCompartidaVendedorRealAsync(int idVendedor, CancellationToken ct = default);

    Task<decimal> RecuperaAnticipoAsync(int idPacienteLocal1, int idPacienteLocal2, int idPacienteLocal3,
        int idPacienteLocal4, int idPacienteLocal5, DateTime fecha, CancellationToken ct = default);

    Task ActualizaDatosTarjetaAsync(int idPaqueteLocal, string tarjetaNumero, string tarjetaFechaVenc, string tarjetaCvv,
        string tipoTarjeta, string tarjetaCs, int tarjetaPrimaria, CancellationToken ct = default);

    Task ActualizaTipoCobranzaAClienteAsync(int idPaqueteLocal, CancellationToken ct = default);

    Task<List<PaqueteServicio>> RecuperarYBorrarAreasPaqueteServicioAsync(int idLocalPaquete, CancellationToken ct = default);

    Task ActualizaPaqueteCobrableConektaAsync(int idPaqueteLocal, int conektaPromocionMsi, CancellationToken ct = default);

    Task<double> RecuperaAnticipoConfiguradoAsync(string montoAPagar, bool esReventa, int idSucursal, string banco, string formaPago, CancellationToken ct = default);
}

public partial class PaqueteService : IPaqueteValidacionesService
{
    public async Task<string> ValidaReventaPacienteConCobranzaRecuperadorAsync(int idPacienteLocal, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "valida_reventa_paciente_con_cobranza_recuperador",
            new Dictionary<string, object?> { ["@id_paciente_local"] = idPacienteLocal }, ct);

        return Convert.ToString(filas.Rows[0]["mensaje"])!;
    }

    public async Task<string> ValidaReventaPacienteAsync(int idPacienteLocal, decimal montoAnticipo, string formaPago,
        string tipoCobranza, DateTime fechaPago1, int idPaqueteLocal, bool ventaLiquidada, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "valida_reventa_paciente",
            new Dictionary<string, object?>
            {
                ["@id_paciente_local"] = idPacienteLocal,
                ["@monto_anticipo"] = montoAnticipo,
                ["@forma_pago"] = formaPago,
                ["@tipo_cobranza"] = tipoCobranza,
                ["@fecha_pago_1"] = Fecha(fechaPago1),
                ["@id_paquete_reventa_inmediata"] = idPaqueteLocal,
                ["@venta_liquidada"] = ventaLiquidada,
            }, ct);

        return Convert.ToString(filas.Rows[0]["mensaje"])!;
    }

    public async Task<string> ValidaReventaSaldoPorPacientesAsync(int idPaciente1, int idPaciente2, int idPaciente3,
        int idPaciente4, int idPaciente5, int idSucursal, bool ventaLiquidada, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "valida_reventa_saldo_por_pacientes",
            new Dictionary<string, object?>
            {
                ["@id_paciente_local_1"] = idPaciente1,
                ["@id_paciente_local_2"] = idPaciente2,
                ["@id_paciente_local_3"] = idPaciente3,
                ["@id_paciente_local_4"] = idPaciente4,
                ["@id_paciente_local_5"] = idPaciente5,
                ["@id_sucursal"] = idSucursal,
                ["@venta_liquidada"] = ventaLiquidada,
            }, ct);

        return Convert.ToString(filas.Rows[0]["mensaje"])!;
    }

    public async Task<int> ValidaPacienteCobranzaCteNoLiquidadoAsync(int idPacienteLocal, CancellationToken ct = default)
    {
        var resultado = await ProcedimientoAlmacenado.EscalarAsync(_db, "valida_paciente_cobranza_cte_no_liquidado",
            new Dictionary<string, object?> { ["@id_paciente_local"] = idPacienteLocal }, ct);

        return Convert.ToInt32(resultado);
    }

    public Task ActualizaPaqueteServicioAsync(int idPaqueteLocal, bool tratLaser, bool tratLaser2, bool tratLaser3,
        bool tratLaser4, bool tratLaser5, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.EscalarAsync(_db, "actualiza_paquete_servicio_areas",
            new Dictionary<string, object?>
            {
                ["@id_paquete_local"] = idPaqueteLocal,
                ["@trat_laser"] = tratLaser,
                ["@trat_laser_2"] = tratLaser2,
                ["@trat_laser_3"] = tratLaser3,
                ["@trat_laser_4"] = tratLaser4,
                ["@trat_laser_5"] = tratLaser5,
            }, ct);

    /// <remarks>Stored procedure <c>recupera_paquete_x_id</c>. El original tronaba
    /// si no había filas; aquí devuelve <c>null</c>.</remarks>
    public async Task<Paquete?> RecuperaPaquetePorIdAsync(int idPaqueteLocal, CancellationToken ct = default)
    {
        var lista = await ProcedimientoAlmacenado.ListaAsync(_db, "recupera_paquete_x_id",
            new Dictionary<string, object?> { ["@id_paquete_local"] = idPaqueteLocal },
            r => new Paquete
            {
                IdLocal = Convert.ToInt32(r["id_local"]),
                FechaModificacionLocal = r.FechaONull("fecha_modificacion_local"),
                FechaInterfaz = r.FechaONull("fecha_interfaz"),
                IdPaquete = r.EnteroONull("id_paquete"),
                IdPacienteLocal = Convert.ToInt32(r["id_paciente_local"]),
                IdPaciente = r.EnteroONull("id_paciente"),
                IdPacienteLocal2 = r.EnteroONull("id_paciente_local_2"),
                IdPaciente2 = r.EnteroONull("id_paciente_2"),
                NombrePaciente1 = r.CadenaODefecto("nombre_paciente_1"),
                NombrePaciente2 = r.CadenaODefecto("nombre_paciente_2"),
                FechaCompra = Convert.ToDateTime(r["fecha_compra"]),
                TratLaser = Convert.ToBoolean(r["trat_laser"]),
                TratCorporal = Convert.ToBoolean(r["trat_corporal"]),
                TratFacial = Convert.ToBoolean(r["trat_facial"]),
                FormaPago = r.CadenaODefecto("forma_pago"),
                TipoCobranza = r.CadenaODefecto("tipo_cobranza"),
                TarjetaNumero = r.CadenaODefecto("tarjeta_numero"),
                TarjetaTipo = r.CadenaODefecto("tarjeta_tipo"),
                TarjetaCvv = r.CadenaODefecto("tarjeta_cvv"),
                TarjetaFechaVenc = r.CadenaODefecto("tarjeta_fecha_venc"),
                CostoTotal = Convert.ToDecimal(r["costo_total"]),
                Anticipo = Convert.ToDecimal(r["anticipo"]),
                PagosXCubrir = r.EnteroONull("pagos_x_cubrir"),
                PagoUnitario = Convert.ToDecimal(r["pago_unitario"]),
                FechaPago1 = Convert.ToDateTime(r["fecha_pago_1"]),
                FechaPago2 = Convert.ToDateTime(r["fecha_pago_2"]),
                FechaPago3 = Convert.ToDateTime(r["fecha_pago_3"]),
                FechaPago4 = Convert.ToDateTime(r["fecha_pago_4"]),
                FechaPago5 = Convert.ToDateTime(r["fecha_pago_5"]),
                FechaPago6 = Convert.ToDateTime(r["fecha_pago_6"]),
                FechaPago7 = Convert.ToDateTime(r["fecha_pago_7"]),
                FechaPago8 = Convert.ToDateTime(r["fecha_pago_8"]),
                FechaPago9 = Convert.ToDateTime(r["fecha_pago_9"]),
                FechaPago10 = Convert.ToDateTime(r["fecha_pago_10"]),
                MontoPago1 = Convert.ToDecimal(r["monto_pago_1"]),
                MontoPago2 = Convert.ToDecimal(r["monto_pago_2"]),
                MontoPago3 = Convert.ToDecimal(r["monto_pago_3"]),
                MontoPago4 = Convert.ToDecimal(r["monto_pago_4"]),
                MontoPago5 = Convert.ToDecimal(r["monto_pago_5"]),
                MontoPago6 = Convert.ToDecimal(r["monto_pago_6"]),
                MontoPago7 = Convert.ToDecimal(r["monto_pago_7"]),
                MontoPago8 = Convert.ToDecimal(r["monto_pago_8"]),
                MontoPago9 = Convert.ToDecimal(r["monto_pago_9"]),
                MontoPago10 = Convert.ToDecimal(r["monto_pago_10"]),
                Observaciones = r.CadenaODefecto("observaciones"),
                IdUsuario = Convert.ToInt32(r["id_usuario"]),
                FechaAlta = Convert.ToDateTime(r["fecha_alta"]),
                TratLaser2 = Convert.ToBoolean(r["trat_laser_2"]),
                TratCorporal2 = Convert.ToBoolean(r["trat_corporal_2"]),
                TratFacial2 = Convert.ToBoolean(r["trat_facial_2"]),
                IdSucursal = Convert.ToInt32(r["id_sucursal"]),
                FechaModificacion = r.FechaONull("fecha_modificacion"),
                IdUsuarioModifica = r.EnteroONull("id_usuario_modifica"),
                PagadoMitad = r.IsDBNull(r.GetOrdinal("pagado_mitad")) ? null : Convert.ToBoolean(r["pagado_mitad"]),
                PagadoMitadFecha = r.FechaONull("pagado_mitad_fecha"),
                MostrarEnSucursales = Convert.ToBoolean(r["mostrar_en_sucursales"]),
                TipoCobranza1 = r.CadenaODefecto("tipo_cobranza_1"),
                TipoCobranza2 = r.CadenaODefecto("tipo_cobranza_2"),
                TipoCobranza3 = r.CadenaODefecto("tipo_cobranza_3"),
                TipoCobranza4 = r.CadenaODefecto("tipo_cobranza_4"),
                TipoCobranza5 = r.CadenaODefecto("tipo_cobranza_5"),
                TipoCobranza6 = r.CadenaODefecto("tipo_cobranza_6"),
                TipoCobranza7 = r.CadenaODefecto("tipo_cobranza_7"),
                TipoCobranza8 = r.CadenaODefecto("tipo_cobranza_8"),
                TipoCobranza9 = r.CadenaODefecto("tipo_cobranza_9"),
                TipoCobranza10 = r.CadenaODefecto("tipo_cobranza_10"),
                IdPromocion = r.EnteroONull("id_promocion"),
                CostoTotalCalculado = Convert.ToDecimal(r["costo_total_calculado"]),
                ClienteReferido = Convert.ToBoolean(r["cliente_referido"]),
                IdClienteRefirio = r.EnteroONull("id_cliente_refirio"),
                TratamientoGratisReferido = Convert.ToBoolean(r["tratamiento_gratis_referido"]),
                IdClienteReferido = r.EnteroONull("id_cliente_referido"),
                IdMedio = r.EnteroONull("id_medio"),
                IdSucursalOrigen = r.EnteroONull("id_sucursal_origen"),
                FechaCobranzaAutomatica = r.FechaONull("fecha_cobranza_automatica"),
                EsReventa = Convert.ToBoolean(r["es_reventa"]),
                NoDisponiblePorMigracion = r.IsDBNull(r.GetOrdinal("no_disponible_por_migracion")) ? null : Convert.ToBoolean(r["no_disponible_por_migracion"]),
                BorradoEnMigracion = r.IsDBNull(r.GetOrdinal("borrado_en_migracion")) ? null : Convert.ToBoolean(r["borrado_en_migracion"]),
                ProvieneDeMigracion = r.IsDBNull(r.GetOrdinal("proviene_de_migracion")) ? null : Convert.ToBoolean(r["proviene_de_migracion"]),
                FechaRecuperacionCobranza = r.FechaONull("fecha_recuperacion_cobranza"),
                FechaCitaUltimatum = r.FechaONull("fecha_cita_ultimatum"),
                TarjetaCs = r.CadenaODefecto("tarjeta_cs"),
                TarjetaNumeroT2 = r.CadenaODefecto("tarjeta_numero_t2"),
                TarjetaTipoT2 = r.CadenaODefecto("tarjeta_tipo_t2"),
                TarjetaCvvT2 = r.CadenaODefecto("tarjeta_cvv_t2"),
                TarjetaFechaVencT2 = r.CadenaODefecto("tarjeta_fecha_venc_t2"),
                TarjetaCsT2 = r.CadenaODefecto("tarjeta_cs_t2"),
                TarjetaPrimaria = r.EnteroONull("tarjeta_primaria"),
                PaqueteCompleto = Convert.ToBoolean(r["paquete_completo"]),
                CompartidoEnfermera = Convert.ToBoolean(r["compartido_enfermera"]),
                IdEnfermera = r.EnteroONull("id_enfermera"),
                IdPacienteLocal3 = r.EnteroONull("id_paciente_local_3"),
                IdPaciente3 = r.EnteroONull("id_paciente_3"),
                NombrePaciente3 = r.CadenaODefecto("nombre_paciente_3"),
                TratLaser3 = Convert.ToBoolean(r["trat_laser_3"]),
                TratCorporal3 = Convert.ToBoolean(r["trat_corporal_3"]),
                TratFacial3 = Convert.ToBoolean(r["trat_facial_3"]),
                IdPacienteLocal4 = r.EnteroONull("id_paciente_local_4"),
                IdPaciente4 = r.EnteroONull("id_paciente_4"),
                NombrePaciente4 = r.CadenaODefecto("nombre_paciente_4"),
                TratLaser4 = Convert.ToBoolean(r["trat_laser_4"]),
                TratCorporal4 = Convert.ToBoolean(r["trat_corporal_4"]),
                TratFacial4 = Convert.ToBoolean(r["trat_facial_4"]),
                IdPacienteLocal5 = r.EnteroONull("id_paciente_local_5"),
                IdPaciente5 = r.EnteroONull("id_paciente_5"),
                NombrePaciente5 = r.CadenaODefecto("nombre_paciente_5"),
                TratLaser5 = Convert.ToBoolean(r["trat_laser_5"]),
                TratCorporal5 = Convert.ToBoolean(r["trat_corporal_5"]),
                TratFacial5 = Convert.ToBoolean(r["trat_facial_5"]),
                SaldoVencido = Convert.ToDecimal(r["saldo_vencido"]),
                SaldoVencido5Dias = Convert.ToDecimal(r["saldo_vencido_5_dias"]),
                SaldoTotal = Convert.ToDecimal(r["saldo_total"]),
                EsNegrita = Convert.ToBoolean(r["es_negrita"]),
                FechaActualizacionSaldos = r.FechaONull("fecha_actualizacion_saldos"),
                SiEnviarPaqueteServicio = r.EnteroONull("si_enviar_paquete_servicio"),
                TokenT1 = r.IsDBNull(r.GetOrdinal("token_t1")) ? null : Convert.ToString(r["token_t1"]),
                TokenT2 = r.IsDBNull(r.GetOrdinal("token_t2")) ? null : Convert.ToString(r["token_t2"]),
                Contrato = r.CadenaODefecto("contrato"),
                FechaSigtePago = r.FechaONull("fecha_sigte_pago"),
                EsEuroskin = r.EnteroONull("es_euroskin"),
            }, ct);

        return lista.FirstOrDefault();
    }

    public Task<DataTable> RecuperaPromocionServiciosDefaultAsync(int idPaquete, int idPaciente, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_promocion_servicios_default",
            new Dictionary<string, object?>
            {
                ["@id_paquete"] = idPaquete,
                ["@id_paciente"] = idPaciente,
            }, ct);

    public Task<DataTable> ValidaAutorizacionVentaAsync(int idPaqueteConsulta, int idPaciente, int idPaciente1, int idPaciente2,
        int idPaciente3, int idPaciente4, int idPaciente5, string pacienteNuevo, string numTarjeta, string numTarjetaT2, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "pos_valida_autorizacion_venta",
            new Dictionary<string, object?>
            {
                ["@id_paquete_consulta"] = idPaqueteConsulta,
                ["@id_paciente"] = idPaciente,
                ["@id_paciente_1"] = idPaciente1,
                ["@id_paciente_2"] = idPaciente2,
                ["@id_paciente_3"] = idPaciente3,
                ["@id_paciente_4"] = idPaciente4,
                ["@id_paciente_5"] = idPaciente5,
                ["@paciente_nuevo"] = pacienteNuevo,
                ["@num_tarjeta"] = numTarjeta,
                ["@num_tarjeta_t2"] = numTarjetaT2,
            }, ct);

    /// <summary>
    /// Detección de cliente repetido por nombre y teléfonos. El original recibía
    /// 25 parámetros sueltos; aquí llegan como listas de hasta 5 pacientes
    /// (posiciones faltantes se envían vacías, igual que el original).
    /// </summary>
    public Task<DataTable> ValidaAutorizacionVentaClienteRepetidoAsync(IReadOnlyList<(string Nombre, string ApPaterno, string ApMaterno)> nombres,
        IReadOnlyList<(string Telefono, string Celular)> telefonos, CancellationToken ct = default)
    {
        static T En<T>(IReadOnlyList<T> lista, int i, T defecto) => i < lista.Count ? lista[i] : defecto;

        var parametros = new Dictionary<string, object?>();
        for (int i = 0; i < 5; i++)
        {
            var n = En(nombres, i, (Nombre: string.Empty, ApPaterno: string.Empty, ApMaterno: string.Empty));
            parametros[$"@p{i + 1}_nombre"] = n.Nombre;
            parametros[$"@p{i + 1}_ap_paterno"] = n.ApPaterno;
            parametros[$"@p{i + 1}_ap_materno"] = n.ApMaterno;
        }

        for (int i = 0; i < 5; i++)
        {
            var t = En(telefonos, i, (Telefono: string.Empty, Celular: string.Empty));
            parametros[$"@p{i + 1}_telefono"] = t.Telefono;
            parametros[$"@p{i + 1}_celular"] = t.Celular;
        }

        return ProcedimientoAlmacenado.TablaAsync(_db, "pos_valida_autorizacion_venta_cliente_repetido", parametros, ct);
    }

    public async Task<int> ValidaTokenAutorizacionVentaAsync(int idPaqueteLocal, string token, string tipo, CancellationToken ct = default)
    {
        var resultado = await ProcedimientoAlmacenado.EscalarAsync(_db, "pos_valida_token_autorizacion_venta",
            new Dictionary<string, object?>
            {
                ["@id_paquete_local"] = idPaqueteLocal,
                ["@token"] = token,
                ["@tipo"] = tipo,
            }, ct);

        return Convert.ToInt32(resultado);
    }

    public async Task<decimal> RecuperaSaldoTotalPaquetesAsync(int idPacienteLocal1, int idPacienteLocal2, int idPacienteLocal3,
        int idPacienteLocal4, int idPacienteLocal5, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "recupera_saldo_total_paquetes",
            new Dictionary<string, object?>
            {
                ["@id_paciente_local_1"] = idPacienteLocal1,
                ["@id_paciente_local_2"] = idPacienteLocal2,
                ["@id_paciente_local_3"] = idPacienteLocal3,
                ["@id_paciente_local_4"] = idPacienteLocal4,
                ["@id_paciente_local_5"] = idPacienteLocal5,
            }, ct);

        return filas.Rows.Count > 0 ? Convert.ToDecimal(filas.Rows[0]["total"]) : 0m;
    }

    public Task<DataTable> ValidarUsarDatosTarjetaAnteriorAsync(int idPacienteLocal, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "validar_usar_datos_tarjeta_anterior",
            new Dictionary<string, object?> { ["@id_paciente_local"] = idPacienteLocal }, ct);

    public Task GuardaTokenAutorizacionVentaAsync(int idPaqueteLocal, string token, string tipo, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.EscalarAsync(_db, "pos_guarda_token_autorizacion_venta",
            new Dictionary<string, object?>
            {
                ["@id_paquete_local"] = idPaqueteLocal,
                ["@token"] = token,
                ["@tipo"] = tipo,
            }, ct);

    public Task<DataTable> ValidaTarjetaNuevoPaqueteAsync(int idPaqueteLocal, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "valida_tarjeta_nuevo_paquete",
            new Dictionary<string, object?> { ["@id_paquete_local"] = idPaqueteLocal }, ct);

    public Task<DataTable> ValidaTarjetaNuevaAsync(string numTarjeta, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "valida_nueva_tarjeta",
            new Dictionary<string, object?> { ["@num_tarjeta"] = numTarjeta }, ct);

    public Task<DataTable> RecuperaReimpresionContratoAsync(CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_paquete_reimpresion_contrato",
            new Dictionary<string, object?>(), ct);

    public async Task<string> ValidaReventaCompartidaVendedorRealAsync(int idVendedor, CancellationToken ct = default)
    {
        var resultado = await ProcedimientoAlmacenado.EscalarAsync(_db, "valida_reventa_compartida_vendedor_real",
            new Dictionary<string, object?> { ["@id_vendedor"] = idVendedor }, ct);

        return Convert.ToString(resultado) ?? string.Empty;
    }

    public async Task<decimal> RecuperaAnticipoAsync(int idPacienteLocal1, int idPacienteLocal2, int idPacienteLocal3,
        int idPacienteLocal4, int idPacienteLocal5, DateTime fecha, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "recupera_anticipo_en_caja",
            new Dictionary<string, object?>
            {
                ["@id_paciente_local_1"] = idPacienteLocal1,
                ["@id_paciente_local_2"] = idPacienteLocal2,
                ["@id_paciente_local_3"] = idPacienteLocal3,
                ["@id_paciente_local_4"] = idPacienteLocal4,
                ["@id_paciente_local_5"] = idPacienteLocal5,
                ["@fecha"] = Fecha(fecha),
            }, ct);

        return filas.Rows.Count > 0 ? Convert.ToDecimal(filas.Rows[0]["monto"]) : 0m;
    }

    public Task ActualizaDatosTarjetaAsync(int idPaqueteLocal, string tarjetaNumero, string tarjetaFechaVenc, string tarjetaCvv,
        string tipoTarjeta, string tarjetaCs, int tarjetaPrimaria, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.EscalarAsync(_db, "actualiza_paquete_datos_tarjeta",
            new Dictionary<string, object?>
            {
                ["@id_paquete_local"] = idPaqueteLocal,
                ["@tarjeta_numero"] = tarjetaNumero,
                ["@tarjeta_fecha_venc"] = tarjetaFechaVenc,
                ["@tarjeta_cvv"] = tarjetaCvv,
                ["@tipo_tarjeta"] = tipoTarjeta,
                ["@tarjeta_cs"] = tarjetaCs,
                ["@tarjeta_primaria"] = tarjetaPrimaria,
            }, ct);

    public Task ActualizaTipoCobranzaAClienteAsync(int idPaqueteLocal, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC actualiza_paquete_tipo_cobranza_a_cliente @id_paquete_local = {idPaqueteLocal}", ct);

    /// <remarks>El SP devuelve las áreas y las borra en la misma llamada (por eso
    /// el nombre); solo materializa id_servicio, id_paciente_local y area_enviada.</remarks>
    public Task<List<PaqueteServicio>> RecuperarYBorrarAreasPaqueteServicioAsync(int idLocalPaquete, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.ListaAsync(_db, "recuperar_y_borrar_areas_paquete_servicio",
            new Dictionary<string, object?> { ["@id_paquete_local"] = idLocalPaquete },
            r => new PaqueteServicio
            {
                IdServicio = Convert.ToInt32(r["id_servicio"]),
                IdPacienteLocal = Convert.ToInt32(r["id_paciente_local"]),
                AreaEnviada = Convert.ToInt32(r["area_enviada"]),
            }, ct);

    public Task ActualizaPaqueteCobrableConektaAsync(int idPaqueteLocal, int conektaPromocionMsi, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_paquete_cobrable_conekta @id_paquete_local = {idPaqueteLocal}, @conekta_promocion_msi = {conektaPromocionMsi}", ct);

    /// <remarks>El original aceptaba el monto como texto (venía de un TextBox)
    /// y lo convertía con CDbl; cadena vacía cuenta como 0.</remarks>
    public async Task<double> RecuperaAnticipoConfiguradoAsync(string montoAPagar, bool esReventa, int idSucursal, string banco, string formaPago, CancellationToken ct = default)
    {
        double monto = string.IsNullOrEmpty(montoAPagar) ? 0d : Convert.ToDouble(montoAPagar);

        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "recupera_anticipo_configurado",
            new Dictionary<string, object?>
            {
                ["@costo_total"] = monto,
                ["@es_reventa_pos"] = esReventa,
                ["@id_sucursal"] = idSucursal,
                ["@banco"] = banco,
                ["@forma_pago"] = formaPago,
            }, ct);

        return filas.Rows.Count > 0 ? Convert.ToDouble(filas.Rows[0]["anticipo_minimo"]) : 0d;
    }
}
