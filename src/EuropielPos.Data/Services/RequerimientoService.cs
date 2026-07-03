using System.Data;
using System.Text.Json;
using EuropielPos.Data.Entities;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>RequerimientoBL.vb</c> (requerimientos/fallas de sucursal).
/// </summary>
public interface IRequerimientoService
{
    Task LimpiarRequerimientoPasoAsync(CancellationToken ct = default);

    Task ProcesaRequerimientoPasoAsync(CancellationToken ct = default);

    Task<DataTable> RecuperaTipoFallaAsync(string texto, string tipoRequerimiento, CancellationToken ct = default);

    Task<DataTable> RecuperaPrioridadAsync(string texto, CancellationToken ct = default);

    Task<DataTable> RecuperaDirigidoAsync(string texto, string bloque, CancellationToken ct = default);

    Task<int> GuardaRequerimientoAsync(string idPrioridad, DateTime? fechaVencimiento, string concepto, decimal monto,
        string dirigido, int idUsuario, string bloqueAlta, string tipo, int idTipoFalla, CancellationToken ct = default);

    Task<DataTable> RecuperaRequerimientosAsync(int idUsuario, string bloque, int esMonitor, string tipo, int idTipoFalla = 0, CancellationToken ct = default);

    Task<string> BorrarRequerimientoAsync(int idRequerimientoLocal, int idUsuario, string bloque, CancellationToken ct = default);

    Task<string> TerminaRequerimientoAsync(int idRequerimientoLocal, int idUsuario, string bloque, CancellationToken ct = default);

    Task<DataTable> ObtenerRequerimientoAsync(int idRequerimientoLocal, CancellationToken ct = default);

    Task<List<Requerimiento>> RecuperaRequerimientoAEnviarAsync(CancellationToken ct = default);

    Task<RequerimientoEnvio> RecuperaRequerimientoEnvioAsync(int idRequerimientoLocal, CancellationToken ct = default);

    Task<string> RecuperaJsonRequerimientoAEnviarAsync(int idRequerimientoLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default);

    Task ActualizaFechaInterfazRequerimientoAsync(int idRequerimientoLocal, DateTime fecha, CancellationToken ct = default);
}

public class RequerimientoService : IRequerimientoService
{
    private readonly PosDbContext _db;

    public RequerimientoService(PosDbContext db) => _db = db;

    public Task LimpiarRequerimientoPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_requerimiento_paso", ct);

    public Task ProcesaRequerimientoPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_requerimiento_paso", ct);

    public Task<DataTable> RecuperaTipoFallaAsync(string texto, string tipoRequerimiento, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_tipo_falla_combo",
            new Dictionary<string, object?>
            {
                ["@texto"] = texto,
                ["@tipo_requerimiento"] = tipoRequerimiento,
            }, ct);

    public Task<DataTable> RecuperaPrioridadAsync(string texto, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_prioridad_combo",
            new Dictionary<string, object?> { ["@texto"] = texto }, ct);

    public Task<DataTable> RecuperaDirigidoAsync(string texto, string bloque, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_dirigido_combo",
            new Dictionary<string, object?>
            {
                ["@texto"] = texto,
                ["@bloque"] = bloque,
            }, ct);

    /// <returns>El id del requerimiento creado, o 0 si el SP no devolvió filas.</returns>
    public async Task<int> GuardaRequerimientoAsync(string idPrioridad, DateTime? fechaVencimiento, string concepto, decimal monto,
        string dirigido, int idUsuario, string bloqueAlta, string tipo, int idTipoFalla, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "guarda_requerimiento",
            new Dictionary<string, object?>
            {
                ["@id_prioridad"] = idPrioridad,
                ["@fecha_vencimiento"] = fechaVencimiento,
                ["@concepto"] = concepto,
                ["@monto"] = monto,
                ["@dirigido"] = dirigido,
                ["@id_usuario"] = idUsuario,
                ["@bloque_alta"] = bloqueAlta,
                ["@tipo"] = tipo,
                ["@id_tipo_falla"] = idTipoFalla,
            }, ct);

        return filas.Rows.Count > 0 ? Convert.ToInt32(filas.Rows[0]["id"]) : 0;
    }

    public Task<DataTable> RecuperaRequerimientosAsync(int idUsuario, string bloque, int esMonitor, string tipo, int idTipoFalla = 0, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_requerimientos",
            new Dictionary<string, object?>
            {
                ["@id_requerimiento_local"] = 0,
                ["@id_usuario"] = idUsuario,
                ["@bloque"] = bloque,
                ["@es_monitor"] = esMonitor,
                ["@tipo"] = tipo,
                ["@id_tipo_falla"] = idTipoFalla,
            }, ct);

    public async Task<string> BorrarRequerimientoAsync(int idRequerimientoLocal, int idUsuario, string bloque, CancellationToken ct = default)
    {
        await _db.Database.ExecuteSqlAsync(
            $"EXEC borrar_requerimiento @id_requerimiento_local = {idRequerimientoLocal}, @id_usuario = {idUsuario}, @bloque = {bloque}", ct);
        return "Ok";
    }

    public async Task<string> TerminaRequerimientoAsync(int idRequerimientoLocal, int idUsuario, string bloque, CancellationToken ct = default)
    {
        await _db.Database.ExecuteSqlAsync(
            $"EXEC termina_requerimiento @id_requerimiento_local = {idRequerimientoLocal}, @id_usuario = {idUsuario}, @bloque = {bloque}", ct);
        return "Ok";
    }

    public Task<DataTable> ObtenerRequerimientoAsync(int idRequerimientoLocal, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_requerimiento",
            new Dictionary<string, object?> { ["@id_requerimiento_local"] = idRequerimientoLocal }, ct);

    /// <remarks>Stored procedure <c>recupera_requerimientos_enviar</c> (pendientes de sincronizar).</remarks>
    public Task<List<Requerimiento>> RecuperaRequerimientoAEnviarAsync(CancellationToken ct = default) =>
        ProcedimientoAlmacenado.ListaAsync(_db, "recupera_requerimientos_enviar",
            new Dictionary<string, object?> { ["@fecha_hoy"] = DateTime.Today },
            r => new Requerimiento
            {
                IdLocal = Convert.ToInt32(r["id_local"]),
                FechaModificacionLocal = r.FechaONull("fecha_modificacion_local"),
                FechaInterfaz = r.FechaONull("fecha_interfaz"),
                IdRequerimiento = r.EnteroONull("id_requerimiento"),
                // OJO: el BL original tomaba id_sucursal como folio (bug histórico
                // que el servidor ya espera); se replica hasta confirmar con el backend.
                Folio = r.IsDBNull(r.GetOrdinal("folio")) ? null : r.EnteroONull("id_sucursal"),
                FechaVencimiento = r.FechaONull("fecha_vencimiento"),
                Concepto = r.CadenaODefecto("concepto"),
                Monto = r.IsDBNull(r.GetOrdinal("monto")) ? null : Convert.ToDecimal(r["monto"]),
                Dirigido = r.CadenaODefecto("dirigido"),
                FechaAlta = r.FechaONull("fecha_alta"),
                IdUsuarioAlta = r.EnteroONull("id_usuario_alta"),
                BloqueAlta = r.CadenaODefecto("bloque_alta"),
                FechaTermino = r.FechaONull("fecha_termino"),
                IdUsuarioTermino = r.EnteroONull("id_usuario_termino"),
                BloqueTermino = r.CadenaODefecto("bloque_termino"),
                EsBorrado = r.IsDBNull(r.GetOrdinal("es_borrado")) ? null : Convert.ToBoolean(r["es_borrado"]),
                IdUsuarioBorro = r.EnteroONull("id_usuario_borro"),
                FechaBorro = r.FechaONull("fecha_borro"),
                BloqueBorro = r.CadenaODefecto("bloque_borro"),
                Tipo = r.CadenaODefecto("tipo"),
                IdTipoFalla = r.EnteroONull("id_tipo_falla"),
            }, ct);

    /// <remarks>Si el requerimiento no existe devuelve un DTO vacío, como el original.</remarks>
    public async Task<RequerimientoEnvio> RecuperaRequerimientoEnvioAsync(int idRequerimientoLocal, CancellationToken ct = default)
    {
        var r = await _db.Requerimiento.AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdLocal == idRequerimientoLocal, ct);

        if (r is null)
            return new RequerimientoEnvio();

        return new RequerimientoEnvio
        {
            id_local = r.IdLocal,
            fecha_modificacion_local = ConfiguraFecha(r.FechaModificacionLocal),
            fecha_interfaz = ConfiguraFecha(r.FechaInterfaz),
            id_requerimiento = r.IdRequerimiento,
            folio = r.Folio,
            fecha_Vencimiento = ConfiguraFecha(r.FechaVencimiento),
            concepto = r.Concepto ?? string.Empty,
            monto = r.Monto ?? 0m,
            dirigido = r.Dirigido ?? string.Empty,
            fecha_alta = ConfiguraFecha(r.FechaAlta),
            id_usuario_alta = r.IdUsuarioAlta,
            bloque_alta = r.BloqueAlta ?? string.Empty,
            fecha_termino = ConfiguraFecha(r.FechaTermino),
            id_usuario_termino = r.IdUsuarioTermino,
            bloque_termino = r.BloqueTermino ?? string.Empty,
            es_borrado = r.EsBorrado ?? false,
            id_usuario_borro = r.IdUsuarioBorro,
            fecha_borro = ConfiguraFecha(r.FechaBorro),
            bloque_borro = r.BloqueBorro ?? string.Empty,
            tipo = r.Tipo ?? string.Empty,
            id_tipo_falla = r.IdTipoFalla,
        };
    }

    /// <summary>
    /// JSON de envío del requerimiento. El original usaba los globales de
    /// <c>modGeneral</c> (bloque/no_caja/id_sucursal); aquí llegan por parámetro.
    /// </summary>
    public async Task<string> RecuperaJsonRequerimientoAEnviarAsync(int idRequerimientoLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default)
    {
        var envio = await RecuperaRequerimientoEnvioAsync(idRequerimientoLocal, ct);
        envio.bloque = bloque;
        envio.no_caja = noCaja;
        envio.id_sucursal = idSucursal;

        return JsonSerializer.Serialize(envio);
    }

    public Task ActualizaFechaInterfazRequerimientoAsync(int idRequerimientoLocal, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_requerimiento @id_requerimiento_local = {idRequerimientoLocal}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss")}", ct);

    private static string? ConfiguraFecha(DateTime? fecha) =>
        fecha?.ToString("yyyy-MM-dd HH:mm:ss");
}
