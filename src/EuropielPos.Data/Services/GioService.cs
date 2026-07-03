using System.Data;
using System.Text.Json;
using EuropielPos.Data.Entities;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>GIOBL.vb</c> (personal GIO: altas, asistencias y sincronización).
/// </summary>
public interface IGioService
{
    Task LimpiarGioPasoAsync(CancellationToken ct = default);

    Task LimpiarGioAsistenciaPasoAsync(CancellationToken ct = default);

    Task ProcesaGioPasoAsync(CancellationToken ct = default);

    Task ProcesaGioAsistenciaPasoAsync(CancellationToken ct = default);

    Task<DataTable> RecuperaTurnoAsync(string tipo, CancellationToken ct = default);

    Task<DataTable> RecuperaGiosPorCiudadAsync(int idSucursal, string texto, CancellationToken ct = default);

    Task<DataTable> RecuperaGioAsync(int idGioLocal, CancellationToken ct = default);

    Task<int> GuardaGioAsync(int idLocal, string nombre, string nombreCuenta, string banco, string claveInterbancaria,
        int idSucursal, string bloque, string telefono, string horario, int idTurno, int esActivo, int idUsuario, CancellationToken ct = default);

    Task<string> GuardaGioAsistenciaAsync(int idLocal, int idSucursal, string bloque, DateTime fecha, int idTurno, string horario, int idUsuario, CancellationToken ct = default);

    Task<List<Gio>> RecuperaGioAEnviarAsync(CancellationToken ct = default);

    Task<GioEnvio> RecuperaGioEnvioAsync(int idGioLocal, CancellationToken ct = default);

    Task<string> RecuperaJsonGioAEnviarAsync(int idGioLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default);

    Task ActualizaFechaInterfazGioAsync(int idGioLocal, DateTime fecha, CancellationToken ct = default);

    Task<List<GioAsistencia>> RecuperaGioAsistenciaAEnviarAsync(CancellationToken ct = default);

    Task<GioAsistenciaEnvio> RecuperaGioAsistenciaEnvioAsync(int idGioAsistenciaLocal, CancellationToken ct = default);

    Task<string> RecuperaJsonGioAsistenciaAEnviarAsync(int idGioAsistenciaLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default);

    Task ActualizaFechaInterfazGioAsistenciaAsync(int idGioAsistenciaLocal, DateTime fecha, CancellationToken ct = default);

    Task<string> ValidaExisteGioAsync(string nombreGio, string nombreCuenta, string clabeInterbancaria, CancellationToken ct = default);

    Task<string> RecuperaNombreGioAsync(int idGio, CancellationToken ct = default);
}

public class GioService : IGioService
{
    private readonly PosDbContext _db;

    public GioService(PosDbContext db) => _db = db;

    public Task LimpiarGioPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_gio_paso", ct);

    public Task LimpiarGioAsistenciaPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_gio_asistencia_paso", ct);

    public Task ProcesaGioPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_gio_paso", ct);

    public Task ProcesaGioAsistenciaPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_gio_asistencia_paso", ct);

    public Task<DataTable> RecuperaTurnoAsync(string tipo, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_gio_turno_combo",
            new Dictionary<string, object?> { ["@tipo"] = tipo }, ct);

    public Task<DataTable> RecuperaGiosPorCiudadAsync(int idSucursal, string texto, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_gios_x_ciudad",
            new Dictionary<string, object?>
            {
                ["@id_sucursal"] = idSucursal,
                ["@texto"] = texto,
            }, ct);

    /// <remarks>El original tenía un catch vacío que ocultaba errores de BD
    /// devolviendo Nothing; aquí la excepción se propaga.</remarks>
    public Task<DataTable> RecuperaGioAsync(int idGioLocal, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_gio",
            new Dictionary<string, object?> { ["@id_gio_local"] = idGioLocal }, ct);

    /// <returns>El id del GIO guardado, o 0 si el SP no devolvió filas.</returns>
    public async Task<int> GuardaGioAsync(int idLocal, string nombre, string nombreCuenta, string banco, string claveInterbancaria,
        int idSucursal, string bloque, string telefono, string horario, int idTurno, int esActivo, int idUsuario, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "guarda_gio",
            new Dictionary<string, object?>
            {
                ["@id_gio_local"] = idLocal,
                ["@nombre"] = nombre,
                ["@nombre_cuenta"] = nombreCuenta,
                ["@clave_interbancaria"] = claveInterbancaria,
                ["@banco"] = banco,
                ["@id_sucursal"] = idSucursal,
                ["@bloque"] = bloque,
                ["@telefono"] = telefono,
                ["@horario"] = horario,
                ["@id_turno"] = idTurno,
                ["@es_activo"] = esActivo,
                ["@id_usuario"] = idUsuario,
            }, ct);

        return filas.Rows.Count > 0 ? Convert.ToInt32(filas.Rows[0]["id"]) : 0;
    }

    /// <returns>El valor de la columna <c>existe</c> del SP, o "No" si no devolvió filas.</returns>
    public async Task<string> GuardaGioAsistenciaAsync(int idLocal, int idSucursal, string bloque, DateTime fecha, int idTurno, string horario, int idUsuario, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "guarda_gio_asistencia",
            new Dictionary<string, object?>
            {
                ["@id_gio_local"] = idLocal,
                ["@id_sucursal"] = idSucursal,
                ["@bloque"] = bloque,
                ["@fecha"] = fecha.ToString("yyyy-MM-dd HH:mm:ss"), // el SP espera texto, como el original
                ["@id_turno"] = idTurno,
                ["@horario"] = horario,
                ["@id_usuario"] = idUsuario,
            }, ct);

        return filas.Rows.Count > 0 ? Convert.ToString(filas.Rows[0]["existe"])! : "No";
    }

    /// <remarks>Stored procedure <c>recupera_gio_enviar</c> (GIOs pendientes de sincronizar).</remarks>
    public Task<List<Gio>> RecuperaGioAEnviarAsync(CancellationToken ct = default) =>
        ProcedimientoAlmacenado.ListaAsync(_db, "recupera_gio_enviar",
            new Dictionary<string, object?> { ["@fecha_hoy"] = DateTime.Today },
            r => new Gio
            {
                IdLocal = Convert.ToInt32(r["id_local"]),
                FechaModificacionLocal = r.FechaONull("fecha_modificacion_local"),
                FechaInterfaz = r.FechaONull("fecha_interfaz"),
                IdGio = r.EnteroONull("id_gio"),
                Nombre = r.CadenaODefecto("nombre"),
                NombreCuenta = r.CadenaODefecto("nombre_cuenta"),
                ClaveInterbancaria = r.CadenaODefecto("clave_interbancaria"),
                Banco = r.CadenaODefecto("banco"),
                Telefono = r.CadenaODefecto("telefono"),
                Horario = r.CadenaODefecto("horario"),
                IdTurno = r.EnteroONull("id_turno"),
                EsActivo = r.IsDBNull(r.GetOrdinal("es_activo")) ? null : Convert.ToBoolean(r["es_activo"]),
                FechaRegistro = r.FechaONull("fecha_registro"),
            }, ct);

    /// <remarks>Si el GIO no existe devuelve un DTO vacío, como el original.</remarks>
    public async Task<GioEnvio> RecuperaGioEnvioAsync(int idGioLocal, CancellationToken ct = default)
    {
        var g = await _db.Gio.AsNoTracking().FirstOrDefaultAsync(x => x.IdLocal == idGioLocal, ct);

        if (g is null)
            return new GioEnvio();

        return new GioEnvio
        {
            id_local = g.IdLocal,
            fecha_modificacion_local = ConfiguraFecha(g.FechaModificacionLocal),
            fecha_interfaz = ConfiguraFecha(g.FechaInterfaz),
            id_gio = g.IdGio,
            nombre = g.Nombre ?? string.Empty,
            nombre_cuenta = g.NombreCuenta ?? string.Empty,
            clave_interbancaria = g.ClaveInterbancaria ?? string.Empty,
            banco = g.Banco ?? string.Empty,
            telefono = g.Telefono ?? string.Empty,
            horario = g.Horario ?? string.Empty,
            id_turno = g.IdTurno,
            es_activo = g.EsActivo ?? false,
            fecha_registro = ConfiguraFecha(g.FechaRegistro),
        };
    }

    public async Task<string> RecuperaJsonGioAEnviarAsync(int idGioLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default)
    {
        var envio = await RecuperaGioEnvioAsync(idGioLocal, ct);
        envio.bloque = bloque;
        envio.no_caja = noCaja;
        envio.id_sucursal = idSucursal;

        return JsonSerializer.Serialize(envio);
    }

    public Task ActualizaFechaInterfazGioAsync(int idGioLocal, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_gio @id_gio_local = {idGioLocal}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss")}", ct);

    /// <remarks>Stored procedure <c>recupera_gio_asistencia_enviar</c>.</remarks>
    public Task<List<GioAsistencia>> RecuperaGioAsistenciaAEnviarAsync(CancellationToken ct = default) =>
        ProcedimientoAlmacenado.ListaAsync(_db, "recupera_gio_asistencia_enviar",
            new Dictionary<string, object?> { ["@fecha_hoy"] = DateTime.Today },
            r => new GioAsistencia
            {
                IdLocal = Convert.ToInt32(r["id_local"]),
                FechaModificacionLocal = r.FechaONull("fecha_modificacion_local"),
                FechaInterfaz = r.FechaONull("fecha_interfaz"),
                IdDetalle = r.EnteroONull("id_detalle"),
                IdGioLocal = Convert.ToInt32(r["id_gio_local"]),
                IdGio = r.EnteroONull("id_gio"),
                Fecha = r.FechaONull("fecha"),
                IdTurno = r.EnteroONull("id_turno"),
                Horario = r.CadenaODefecto("horario"),
                IdUsuarioRegistro = r.EnteroONull("id_usuario_registro"),
                FechaRegistro = r.FechaONull("fecha_registro"),
            }, ct);

    /// <remarks>Si la asistencia no existe devuelve un DTO vacío, como el original.</remarks>
    public async Task<GioAsistenciaEnvio> RecuperaGioAsistenciaEnvioAsync(int idGioAsistenciaLocal, CancellationToken ct = default)
    {
        var a = await _db.GioAsistencia.AsNoTracking().FirstOrDefaultAsync(x => x.IdLocal == idGioAsistenciaLocal, ct);

        if (a is null)
            return new GioAsistenciaEnvio();

        return new GioAsistenciaEnvio
        {
            id_local = a.IdLocal,
            fecha_modificacion_local = ConfiguraFecha(a.FechaModificacionLocal),
            fecha_interfaz = ConfiguraFecha(a.FechaInterfaz),
            id_detalle = a.IdDetalle,
            id_gio_local = a.IdGioLocal ?? 0,
            id_gio = a.IdGio,
            fecha = ConfiguraFecha(a.Fecha),
            id_turno = a.IdTurno ?? 0,
            horario = a.Horario ?? string.Empty,
            id_usuario_registro = a.IdUsuarioRegistro,
            fecha_registro = ConfiguraFecha(a.FechaRegistro),
        };
    }

    public async Task<string> RecuperaJsonGioAsistenciaAEnviarAsync(int idGioAsistenciaLocal, string bloque, int noCaja, int idSucursal, CancellationToken ct = default)
    {
        var envio = await RecuperaGioAsistenciaEnvioAsync(idGioAsistenciaLocal, ct);
        envio.bloque = bloque;
        envio.no_caja = noCaja;
        envio.id_sucursal = idSucursal;

        return JsonSerializer.Serialize(envio);
    }

    public Task ActualizaFechaInterfazGioAsistenciaAsync(int idGioAsistenciaLocal, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_gio_asistencia @id_gio_asistencia_local = {idGioAsistenciaLocal}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss")}", ct);

    /// <remarks>El original concatenaba los valores en el texto del SP (inyección SQL);
    /// aquí se ejecuta parametrizado.</remarks>
    public async Task<string> ValidaExisteGioAsync(string nombreGio, string nombreCuenta, string clabeInterbancaria, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "valida_existencia_gio",
            new Dictionary<string, object?>
            {
                ["@nombre_gio"] = nombreGio,
                ["@nombre_cuenta"] = nombreCuenta,
                ["@clabe_interbancaria"] = clabeInterbancaria,
            }, ct);

        return filas.Rows.Count > 0 ? Convert.ToString(filas.Rows[0]["existe"])! : "No";
    }

    public async Task<string> RecuperaNombreGioAsync(int idGio, CancellationToken ct = default)
    {
        var nombre = await _db.Gio.AsNoTracking()
            .Where(g => g.IdGio == idGio)
            .Select(g => g.Nombre)
            .FirstOrDefaultAsync(ct);

        return nombre ?? string.Empty;
    }

    private static string? ConfiguraFecha(DateTime? fecha) =>
        fecha?.ToString("yyyy-MM-dd HH:mm:ss");
}
