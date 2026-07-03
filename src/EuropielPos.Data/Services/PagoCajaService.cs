using System.Data;
using System.Text;
using System.Text.Json;
using EuropielPos.Data.Entities;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>PagoCajaBL.vb</c> — el flujo de dinero de la caja: cobros,
/// formas de pago, folios de recibo y sincronización de pagos.
/// Los valores que el original tomaba de <c>modGeneral</c> (bloque, no_caja,
/// id_sucursal, es_euroskin) llegan por parámetro.
/// </summary>
public interface IPagoCajaService
{
    Task ActualizaFechaInterfazPagoCajaAsync(int idPagoLocal, DateTime fecha, CancellationToken ct = default);

    Task<string> RecuperaJsonPagoCajaAEnviarAsync(PagoCaja pagoCaja, string bloque, int noCaja, int idSucursal, CancellationToken ct = default);

    Task<PagoCajaFormaEnvio> RecuperaPagoFormaAsync(int idPagoLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default);

    Task<List<PagoCajaDetalleEnvio>> RecuperaPagoDetalleAsync(int idPagoLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default);

    Task<List<PagoCaja>> RecuperaPagoCajaAEnviarAsync(CancellationToken ct = default);

    Task<DataTable> RecuperaAnticipoSinAsignarAsync(CancellationToken ct = default);

    Task ActualizaAnticipoSueltoAsync(int idPagoLocal, CancellationToken ct = default);

    Task<PagoCaja?> RecuperaPagoCajaPorIdAsync(int idPagoCaja, CancellationToken ct = default);

    Task<PagoCajaForma?> RecuperaPagoCajaFormaPorIdPagoAsync(int idPagoCaja, CancellationToken ct = default);

    Task<PagoCaja?> RecuperaPagoCajaPorClienteYMontoAsync(int idPacienteLocal, DateTime fecha, decimal monto, CancellationToken ct = default);

    Task<int> GuardaPagoCajaAsync(int idUsuario, string tipoRecibo, string nombre, string domicilio, string rfc, decimal subtotal,
        decimal iva, decimal total, decimal pago, int folioRecibo, int idPacienteLocal, int idPaciente,
        int guardaPaciente, int idBanco, int idSucursal, int esAnticipo, decimal pagoEfectivo,
        decimal pagoTc, decimal pagoTd, decimal pagoTransferencia, int idProducto, int idPaquete, int esEuroskin, CancellationToken ct = default);

    Task ActualizaPacienteEnPagoCajaAsync(int idPago, int idPacienteLocal, int idPaciente, string nombre, CancellationToken ct = default);

    Task ActualizaNombrePacienteEnPagoCajaAsync(int idPago, string nombre, CancellationToken ct = default);

    Task<decimal> RecuperaTotalPagosPorClienteAsync(int idPacienteLocal, CancellationToken ct = default);

    Task<decimal> RecuperaTotalPagadoHoyPorClienteAsync(int idPacienteLocal, CancellationToken ct = default);

    Task<int> RecuperaSigteFolioAsync(int esEuroskin, string formaPago = "", CancellationToken ct = default);

    Task<decimal> ObtenerAnticipoMinimoAsync(double costoPaquete, int esReventa, int esProtegido, CancellationToken ct = default);
}

public class PagoCajaService : IPagoCajaService
{
    private readonly PosDbContext _db;

    public PagoCajaService(PosDbContext db) => _db = db;

    public Task ActualizaFechaInterfazPagoCajaAsync(int idPagoLocal, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_pago_caja @id_pago_local = {idPagoLocal}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss")}", ct);

    /// <summary>
    /// Fragmento JSON del pago con su forma y detalle. OJO: igual que el
    /// original, el fragmento abre con <c>{"pago_caja":</c> pero NO cierra la
    /// llave exterior — el llamador del flujo de sincronización la completa.
    /// </summary>
    public async Task<string> RecuperaJsonPagoCajaAEnviarAsync(PagoCaja pagoCaja, string bloque, int noCaja, int idSucursal, CancellationToken ct = default)
    {
        var pc = new PagoCajaEnvio
        {
            bloque = bloque,
            no_caja = noCaja,
            // El original asignaba modGeneral.IdSucursal y luego lo sobreescribía
            // con el id_sucursal del propio pago; se conserva el valor del pago.
            id_sucursal = pagoCaja.IdSucursal ?? 0,
            id_local = pagoCaja.IdLocal,
            fecha_modificacion_local = ConfiguraFecha(pagoCaja.FechaModificacionLocal),
            fecha_interfaz = ConfiguraFecha(pagoCaja.FechaInterfaz),
            id_pago = pagoCaja.IdPago,
            fecha = ConfiguraFecha(pagoCaja.Fecha),
            id_usuario = pagoCaja.IdUsuario,
            tipo_recibo = pagoCaja.TipoRecibo,
            nombre = pagoCaja.Nombre,
            domicilio = pagoCaja.Domicilio,
            rfc = pagoCaja.Rfc,
            subtotal = pagoCaja.Subtotal,
            iva = pagoCaja.Iva,
            total = pagoCaja.Total,
            pago = pagoCaja.Pago,
            FOLIO_RECIBO = pagoCaja.FolioRecibo,
            id_paciente_local = pagoCaja.IdPacienteLocal,
            id_paciente = pagoCaja.IdPaciente,
            fecha_alta = ConfiguraFecha(pagoCaja.FechaAlta),
            id_banco = pagoCaja.IdBanco,
            es_anticipo = pagoCaja.EsAnticipo,
            folio_facturacion = pagoCaja.FolioFacturacion,
            es_euroskin = pagoCaja.EsEuroskin,
        };

        var forma = await RecuperaPagoFormaAsync(pagoCaja.IdLocal, bloque, noCaja, idSucursal, ct);
        var detalle = await RecuperaPagoDetalleAsync(pagoCaja.IdLocal, bloque, noCaja, idSucursal, ct);

        var json = new StringBuilder();
        json.Append("{\"pago_caja\": ").Append(JsonSerializer.Serialize(pc));
        json.Append(",\"pago_caja_forma\": ").Append(JsonSerializer.Serialize(forma));
        json.Append(",\"pago_caja_detalle\": ").Append(JsonSerializer.Serialize(detalle));

        return json.ToString();
    }

    /// <remarks>El original iteraba las filas reutilizando el mismo objeto,
    /// por lo que devolvía la última; normalmente la relación es 1:1.</remarks>
    public async Task<PagoCajaFormaEnvio> RecuperaPagoFormaAsync(int idPagoLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default)
    {
        var filas = await _db.PagoCajaForma.AsNoTracking()
            .Where(f => f.IdPagoLocal == idPagoLocal)
            .ToListAsync(ct);

        var ps = new PagoCajaFormaEnvio();

        foreach (var f in filas)
        {
            ps.bloque = bloque;
            ps.id_sucursal = idSucursal;
            ps.no_caja = noCaja;
            ps.id_local = f.IdLocal;
            ps.fecha_modificacion_local = ConfiguraFecha(f.FechaModificacionLocal);
            ps.fecha_interfaz = ConfiguraFecha(f.FechaInterfaz);
            ps.id_pago_local = f.IdPagoLocal;
            ps.id_pago = f.IdPago;
            ps.pago = f.Pago ?? 0m;
            ps.pago_efectivo = f.PagoEfectivo ?? 0m;
            ps.pago_tc = f.PagoTc ?? 0m;
            ps.pago_td = f.PagoTd ?? 0m;
            // El original convertía estos dos con Convert.ToInt32 (pierde los
            // centavos en el payload); se replica hasta validar con el backend.
            ps.pago_sin_categoria = f.PagoSinCategoria is null ? null : Convert.ToInt32(f.PagoSinCategoria.Value);
            ps.pago_ca = f.PagoCa is null ? null : Convert.ToInt32(f.PagoCa.Value);
            ps.pago_transferencia = f.PagoTransferencia;
        }

        return ps;
    }

    public async Task<List<PagoCajaDetalleEnvio>> RecuperaPagoDetalleAsync(int idPagoLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default)
    {
        var filas = await _db.PagoCajaDetalle.AsNoTracking()
            .Where(d => d.IdPagoLocal == idPagoLocal)
            .ToListAsync(ct);

        return filas.Select(d => new PagoCajaDetalleEnvio
        {
            bloque = bloque,
            id_sucursal = idSucursal,
            no_caja = noCaja,
            id_local = d.IdLocal,
            fecha_modificacion_local = ConfiguraFecha(d.FechaModificacionLocal),
            fecha_interfaz = ConfiguraFecha(d.FechaInterfaz),
            id_pago_detalle = d.IdPagoDetalle,
            id_pago_local = d.IdPagoLocal,
            id_pago = d.IdPago,
            id_producto = d.IdProducto ?? 0,
            monto = d.Monto ?? 0m,
            id_local_paciente = d.IdLocalPaciente ?? 0,
            id_paciente = d.IdPaciente,
            id_paquete_local = d.IdPaqueteLocal ?? 0,
            id_paquete = d.IdPaquete,
            pago_recuperacion = d.PagoRecuperacion ?? 0,
            id_recuperador = d.IdRecuperador,
        }).ToList();
    }

    /// <remarks>Pagos pendientes de sincronizar con paciente asignado.</remarks>
    public Task<List<PagoCaja>> RecuperaPagoCajaAEnviarAsync(CancellationToken ct = default) =>
        _db.PagoCaja.AsNoTracking()
            .Where(p => p.FechaInterfaz == null && p.IdPacienteLocal != null)
            .ToListAsync(ct);

    public Task<DataTable> RecuperaAnticipoSinAsignarAsync(CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_anticipo_sin_asignar",
            new Dictionary<string, object?>(), ct);

    public Task ActualizaAnticipoSueltoAsync(int idPagoLocal, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC actualiza_pago_caja_anticipo_suelto @id_pago_local = {idPagoLocal}", ct);

    /// <remarks>El original nunca poblaba <c>id_pago</c> en este método (variable
    /// declarada pero jamás leída de la fila); se replica ese comportamiento.
    /// Devuelve <c>null</c> si no existe (el original devolvía un objeto vacío).</remarks>
    public async Task<PagoCaja?> RecuperaPagoCajaPorIdAsync(int idPagoCaja, CancellationToken ct = default)
    {
        var pago = await _db.PagoCaja.AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdLocal == idPagoCaja, ct);

        if (pago is not null)
        {
            pago.IdPago = null; // comportamiento del BL original
            pago.EsEuroskin ??= 0;
        }

        return pago;
    }

    public Task<PagoCajaForma?> RecuperaPagoCajaFormaPorIdPagoAsync(int idPagoCaja, CancellationToken ct = default) =>
        _db.PagoCajaForma.AsNoTracking()
            .FirstOrDefaultAsync(f => f.IdPagoLocal == idPagoCaja, ct);

    /// <remarks>Stored procedure <c>recupera_pago_caja_x_cliente_y_monto</c>.</remarks>
    public async Task<PagoCaja?> RecuperaPagoCajaPorClienteYMontoAsync(int idPacienteLocal, DateTime fecha, decimal monto, CancellationToken ct = default)
    {
        var lista = await ProcedimientoAlmacenado.ListaAsync(_db, "recupera_pago_caja_x_cliente_y_monto",
            new Dictionary<string, object?>
            {
                ["@id_paciente_local"] = idPacienteLocal,
                ["@fecha"] = fecha,
                ["@monto"] = monto,
            },
            r => new PagoCaja
            {
                IdLocal = Convert.ToInt32(r["id_local"]),
                FechaModificacionLocal = r.FechaONull("fecha_modificacion_local"),
                FechaInterfaz = r.FechaONull("fecha_interfaz"),
                IdPago = r.EnteroONull("id_pago"),
                Fecha = Convert.ToDateTime(r["fecha"]),
                IdUsuario = Convert.ToInt32(r["id_usuario"]),
                TipoRecibo = r.CadenaODefecto("tipo_recibo"),
                Nombre = r.CadenaODefecto("nombre"),
                Domicilio = r.CadenaODefecto("domicilio"),
                Rfc = r.CadenaODefecto("rfc"),
                Subtotal = Convert.ToDecimal(r["subtotal"]),
                Iva = Convert.ToDecimal(r["iva"]),
                Total = Convert.ToDecimal(r["total"]),
                Pago = Convert.ToDecimal(r["pago"]),
                FolioRecibo = Convert.ToInt32(r["FOLIO_RECIBO"]),
                IdPacienteLocal = Convert.ToInt32(r["id_paciente_local"]),
                IdPaciente = Convert.ToInt32(r["id_paciente"]),
                FechaAlta = Convert.ToDateTime(r["fecha_alta"]),
                IdBanco = Convert.ToInt32(r["id_banco"]),
                IdSucursal = Convert.ToInt32(r["id_sucursal"]),
                EsAnticipo = Convert.ToInt32(r["es_anticipo"]),
                FolioFacturacion = r.CadenaODefecto("folio_facturacion"),
            }, ct);

        return lista.FirstOrDefault();
    }

    /// <remarks>Stored procedure <c>guarda_pago</c>; devuelve el id del pago creado.</remarks>
    public async Task<int> GuardaPagoCajaAsync(int idUsuario, string tipoRecibo, string nombre, string domicilio, string rfc, decimal subtotal,
        decimal iva, decimal total, decimal pago, int folioRecibo, int idPacienteLocal, int idPaciente,
        int guardaPaciente, int idBanco, int idSucursal, int esAnticipo, decimal pagoEfectivo,
        decimal pagoTc, decimal pagoTd, decimal pagoTransferencia, int idProducto, int idPaquete, int esEuroskin, CancellationToken ct = default)
    {
        var resultado = await ProcedimientoAlmacenado.EscalarAsync(_db, "guarda_pago",
            new Dictionary<string, object?>
            {
                ["@id_usuario"] = idUsuario,
                ["@tipo_recibo"] = tipoRecibo,
                ["@nombre"] = nombre,
                ["@domicilio"] = domicilio,
                ["@rfc"] = rfc,
                ["@subtotal"] = subtotal,
                ["@iva"] = iva,
                ["@total"] = total,
                ["@pago"] = pago,
                ["@folio_recibo"] = folioRecibo,
                ["@id_paciente_local"] = idPacienteLocal,
                ["@id_paciente"] = idPaciente,
                ["@guarda_paciente"] = guardaPaciente,
                ["@id_banco"] = idBanco,
                ["@id_sucursal"] = idSucursal,
                ["@es_anticipo"] = esAnticipo,
                ["@pago_efectivo"] = pagoEfectivo,
                ["@pago_tc"] = pagoTc,
                ["@pago_td"] = pagoTd,
                ["@pago_transferencia"] = pagoTransferencia,
                ["@id_producto"] = idProducto,
                ["@id_paquete"] = idPaquete,
                ["@es_euroskin"] = esEuroskin,
            }, ct);

        return Convert.ToInt32(resultado);
    }

    public Task ActualizaPacienteEnPagoCajaAsync(int idPago, int idPacienteLocal, int idPaciente, string nombre, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_paciente_en_pago @id_pago = {idPago}, @id_paciente_local = {idPacienteLocal}, @id_paciente = {idPaciente}, @nombre = {nombre}", ct);

    public Task ActualizaNombrePacienteEnPagoCajaAsync(int idPago, string nombre, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_nombre_paciente_en_pago @id_pago = {idPago}, @nombre = {nombre}", ct);

    public async Task<decimal> RecuperaTotalPagosPorClienteAsync(int idPacienteLocal, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "recupera_total_pagos_x_cliente",
            new Dictionary<string, object?> { ["@id_paciente_local"] = idPacienteLocal }, ct);

        return filas.Rows.Count > 0 ? Convert.ToDecimal(filas.Rows[0]["total_pagos"]) : 0m;
    }

    public async Task<decimal> RecuperaTotalPagadoHoyPorClienteAsync(int idPacienteLocal, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "recupera_total_pagado_hoy_x_cliente",
            new Dictionary<string, object?> { ["@id_paciente_local"] = idPacienteLocal }, ct);

        return filas.Rows.Count > 0 ? Convert.ToDecimal(filas.Rows[0]["total_pagos"]) : 0m;
    }

    /// <summary>
    /// Siguiente folio de recibo. Serie separada para transferencias
    /// (base 999000) y por marca es_euroskin (base 1000).
    /// </summary>
    public async Task<int> RecuperaSigteFolioAsync(int esEuroskin, string formaPago = "", CancellationToken ct = default)
    {
        int folio;

        if (formaPago == "T")
        {
            folio = await _db.PagoCaja
                .Where(p => p.Rfc == "TRANSFERENCIA")
                .MaxAsync(p => (int?)p.FolioRecibo, ct) ?? 0;
        }
        else
        {
            // La comparación explícita con null replica el <> de SQL, que
            // excluye los rfc NULL (EF Core los incluiría por compensación de nulls).
            int marca = esEuroskin == 1 ? 1 : 0;
            folio = await _db.PagoCaja
                .Where(p => p.Rfc != null && p.Rfc != "TRANSFERENCIA" && p.EsEuroskin == marca)
                .MaxAsync(p => (int?)p.FolioRecibo, ct) ?? 0;
        }

        if (folio == 0)
            folio = formaPago == "T" ? 999000 : 1000;

        return folio + 1;
    }

    /// <remarks>Stored procedure <c>obtener_anticipo_minimo</c>.</remarks>
    public async Task<decimal> ObtenerAnticipoMinimoAsync(double costoPaquete, int esReventa, int esProtegido, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "obtener_anticipo_minimo",
            new Dictionary<string, object?>
            {
                ["@costo_paquete"] = costoPaquete,
                ["@es_reventa"] = esReventa,
                ["@es_protegido"] = esProtegido,
            }, ct);

        return filas.Rows.Count > 0 ? Convert.ToDecimal(filas.Rows[0]["anticipo_minimo"]) : 0m;
    }

    private static string? ConfiguraFecha(DateTime? fecha) =>
        fecha?.ToString("yyyy-MM-dd HH:mm:ss");
}
