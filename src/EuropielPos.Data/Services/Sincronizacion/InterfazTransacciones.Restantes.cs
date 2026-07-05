using System.Text.Json;
using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Bloque 4b del port de <c>modInterfaz.vb</c>: GIOs, citas, pacientes
/// incrementales, paquete_servicio, eliminados, máquinas láser, parámetros
/// generales y consultas para UI (históricos, alertas, mensajes, indicadores).
/// </summary>
public interface IInterfazTransaccionesRestantes
{
    Task<string> GetGiosCargaInicialAsync(CancellationToken ct = default);

    Task<string> GetGiosAsync(CancellationToken ct = default);

    Task<string> GetGiosAsistenciaAsync(CancellationToken ct = default);

    Task<string> GetPacientesAsync(CancellationToken ct = default);

    Task<string> GetPaqueteServicioCargaInicialAsync(CancellationToken ct = default);

    Task<string> GetPaqueteServicioAsync(CancellationToken ct = default);

    Task<string> GetCitasCargaInicialAsync(CancellationToken ct = default);

    Task<string> GetCitasAsync(CancellationToken ct = default);

    Task<string> GetCitasAFuturoAsync(bool borrarCitasAFuturo, CancellationToken ct = default);

    Task<string> GetCitasBorradasAsync(CancellationToken ct = default);

    Task<string> GetPacientesEliminadosAsync(CancellationToken ct = default);

    Task<string> GetMaquinasLaserDetalleAsync(CancellationToken ct = default);

    Task<string> GetParametrosGeneralAsync(CancellationToken ct = default);

    Task<string> GetPaquetesEliminadosAsync(CancellationToken ct = default);

    /// <summary>Histórico de citas de un paciente (asistidas, borradas). Se
    /// devuelven como JSON crudo; se tiparán al portar la pantalla que los muestra.</summary>
    Task<(JsonElement Asistidas, JsonElement Borradas)> CitasHistoricoAsync(int claveBloque, int idPaciente, int idSucursal, CancellationToken ct = default);

    /// <summary>Cobranza histórica de un paquete (detalle, otros paquetes,
    /// plan de pagos, histórico y consolidado de caja).</summary>
    Task<(JsonElement DetallePaquete, JsonElement OtrosPaquetes, JsonElement PlanPagos, JsonElement HistoricoCaja, JsonElement ConsolidadoCaja)>
        CobranzaHistoricoAsync(int claveBloque, int idPaquete, CancellationToken ct = default);

    Task<JsonElement> GetAlertasAsync(int idUsuario, CancellationToken ct = default);

    Task<JsonElement> GetMensajesAsync(string bloque, string usuarioLogin, CancellationToken ct = default);

    Task<JsonElement> GetIndicadoresUsuarioAsync(int idUsuario, CancellationToken ct = default);
}

public partial class InterfazTransaccionesService : IInterfazTransaccionesRestantes
{
    public async Task<string> GetGiosCargaInicialAsync(CancellationToken ct = default)
    {
        try
        {
            var valor = await DescargarValorAsync("/api/europielpos/GetGIOSCA", string.Empty,
                TimeoutCargaInicial, conIdBloque: false, ct);

            if (valor.TryGetProperty("GIO", out var gios))
            {
                await BulkJson.BulkInsertAsync(_db, "gio", gios,
                    new Dictionary<string, object?> { ["fecha_interfaz"] = DateTime.Now }, ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            // (sic) el original reportaba "GetPaquetesCargaInicial" por copy-paste
            throw new Exception("Error HttpPost_GetGIOSCargaInicial: " + ex.Message, ex);
        }
    }

    public Task<string> GetGiosAsync(CancellationToken ct = default) =>
        IncrementalPasoAsync("recupera_gio", "carga_inicial_gios", "/api/europielpos/GetGIOS",
            CuerpoSucursalFecha, "GIOS", "gio_paso",
            limpiar: c => _db.Database.ExecuteSqlAsync($"EXEC limpiar_gio_paso", c),
            procesar: c => _db.Database.ExecuteSqlAsync($"EXEC procesa_gio_paso", c),
            "HttpPost_GetGIO", conIdBloque: false, timeout: TimeoutIncremental, ct: ct);

    public Task<string> GetGiosAsistenciaAsync(CancellationToken ct = default) =>
        IncrementalPasoAsync("recupera_gio_asistencia", "carga_inicial_gios_asistencia", "/api/europielpos/GetGIOS_Asistencia",
            CuerpoSucursalFecha, "GIOS_Asistencia", "gio_asistencia_paso",
            limpiar: c => _db.Database.ExecuteSqlAsync($"EXEC limpiar_gio_asistencia_paso", c),
            procesar: c => _db.Database.ExecuteSqlAsync($"EXEC procesa_gio_asistencia_paso", c),
            "HttpPost_GetGIO", conIdBloque: false, timeout: TimeoutIncremental, ct: ct);

    /// <remarks>Quirk del original conservado: la sucursal 51 del bloque 14
    /// usa timeout de 10s en lugar de 50s.</remarks>
    public Task<string> GetPacientesAsync(CancellationToken ct = default)
    {
        int timeout = _contexto.IdSucursal == 51 && _contexto.IdBloque == 14 ? 10 : 50;

        return IncrementalPasoAsync("recupera_paciente", "carga_inicial_paciente", "/api/europielpos/GetPacientesNuevo",
            CuerpoSucursalFecha, "Pacientes", "paciente_paso",
            limpiar: c => _db.Database.ExecuteSqlAsync($"EXEC limpiar_paciente_paso", c),
            procesar: c => _db.Database.ExecuteSqlAsync($"EXEC procesa_paciente_paso", c),
            "HttpPost_GetPacientes", conIdBloque: true, timeout: timeout, ct: ct);
    }

    public async Task<string> GetPaqueteServicioCargaInicialAsync(CancellationToken ct = default)
    {
        try
        {
            var valor = await DescargarValorAsync("/api/europielpos/GetPaqueteServicioCA", string.Empty,
                TimeoutCargaInicial, conIdBloque: true, ct);

            if (valor.TryGetProperty("PaqueteServicio", out var servicios))
            {
                await BulkJson.BulkInsertAsync(_db, "paquete_servicio", servicios,
                    new Dictionary<string, object?> { ["fecha_interfaz"] = DateTime.Now }, ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetPaqueteServicioCargaInicial: " + ex.Message, ex);
        }
    }

    public Task<string> GetPaqueteServicioAsync(CancellationToken ct = default) =>
        IncrementalPasoAsync("recupera_paquete_servicio", "carga_inicial_paquete_servicio", "/api/europielpos/GetPaqueteServicio",
            f => $"{{ \"Fecha\": \"{Fecha(f)}\" }}", "PaqueteServicioPaso", "paquete_servicio_paso",
            limpiar: c => _db.Database.ExecuteSqlAsync($"EXEC limpiar_paquete_servicio_paso", c),
            procesar: c => _db.Database.ExecuteSqlAsync($"EXEC procesa_paquete_servicio_paso", c),
            "HttpPost_GetPaqueteServicio", conIdBloque: true, timeout: 500, ct: ct);

    public async Task<string> GetCitasCargaInicialAsync(CancellationToken ct = default)
    {
        try
        {
            var valor = await DescargarValorAsync("/api/europielpos/GetCitasCA",
                $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\" }}", TimeoutCargaInicial, conIdBloque: true, ct);

            if (valor.TryGetProperty("Citas", out var citas))
            {
                await BulkJson.BulkInsertAsync(_db, "cita", citas,
                    new Dictionary<string, object?>
                    {
                        ["fecha_interfaz"] = DateTime.Now,
                        ["estatus_interfaz"] = "C",
                    }, ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetCitasCargaInicial: " + ex.Message, ex);
        }
    }

    public Task<string> GetCitasAsync(CancellationToken ct = default) =>
        IncrementalPasoAsync("recupera_cita", "carga_inicial_cita", "/api/europielpos/GetCitasV2",
            CuerpoSucursalFecha, "Citas", "cita_paso",
            limpiar: c => _db.Database.ExecuteSqlAsync($"EXEC limpiar_cita_paso", c),
            procesar: c => _db.Database.ExecuteSqlAsync($"EXEC procesa_cita_paso", c),
            "HttpPost_GetCitas", conIdBloque: true, timeout: 150, ajustarFechaConMax: true, ct: ct);

    public async Task<string> GetCitasAFuturoAsync(bool borrarCitasAFuturo, CancellationToken ct = default)
    {
        try
        {
            DateTime fecha = await _logInterfaz.RecuperaFechaLogInterfazAsync("recupera_cita_a_futuro", "recupera_cita_a_futuro", ct)
                             ?? HoyALas8();

            var valor = await DescargarValorAsync("/api/europielpos/GetCitasAFuturo",
                $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\", \"IdBloque\": \"{_contexto.IdBloque}\" }}",
                600, conIdBloque: true, ct);

            DateTime fechaServidor = LeerFechaServidor(valor);

            await _db.Database.ExecuteSqlAsync($"EXEC limpiar_cita_paso", ct);

            if (valor.TryGetProperty("Citas", out var paso) && paso.GetArrayLength() > 0)
            {
                if (borrarCitasAFuturo)
                    await _db.Database.ExecuteSqlAsync($"EXEC borrar_citas_a_futuro", ct);

                await BulkJson.BulkInsertAsync(_db, "cita_paso", paso, ct: ct);
                await _db.Database.ExecuteSqlAsync($"EXEC procesa_cita_paso", ct);

                fechaServidor = AjustaFechaServidor(paso, fecha, fechaServidor);
            }

            return "OK|" + Fecha(fechaServidor);
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetCitasAFuturo: " + ex.Message, ex);
        }
    }

    /// <remarks>Las citas borradas se marcan con un número de lote
    /// (max(procesada)+1) y el SP procesa solo ese lote.</remarks>
    public async Task<string> GetCitasBorradasAsync(CancellationToken ct = default)
    {
        try
        {
            DateTime fecha = await _logInterfaz.RecuperaFechaLogInterfazAsync("recupera_cita_borrada", "carga_inicial_cita", ct)
                             ?? HoyALas8();

            var valor = await DescargarValorAsync("/api/europielpos/GetCitasBorradas",
                CuerpoSucursalFecha(fecha), TimeoutIncremental, conIdBloque: true, ct);

            DateTime fechaServidor = LeerFechaServidor(valor);

            if (valor.TryGetProperty("Citas", out var borradas) && borradas.GetArrayLength() > 0)
            {
                int lote = (await _db.CitaBorrada.MaxAsync(x => (int?)x.Procesada, ct) ?? 0) + 1;

                await BulkJson.BulkInsertAsync(_db, "cita_borrada", borradas,
                    new Dictionary<string, object?> { ["procesada"] = lote }, ct);

                await _db.Database.ExecuteSqlAsync($"EXEC procesa_cita_borrada @procesada = {lote}", ct);
            }

            return "OK|" + Fecha(fechaServidor);
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetCitasBorradas: " + ex.Message, ex);
        }
    }

    /// <remarks>Este método registra su propia corrida en log_interfaz.</remarks>
    public async Task<string> GetPacientesEliminadosAsync(CancellationToken ct = default)
    {
        try
        {
            DateTime fecha = await _logInterfaz.RecuperaFechaLogInterfazAsync("recupera_pacientes_eliminados", string.Empty, ct)
                             ?? HoyALas8();

            int idLog = await _logInterfaz.GuardaInicioLogInterfazAsync("recupera_pacientes_eliminados", fecha, ct);

            var valor = await DescargarValorAsync("/api/europielpos/GetPacientesEliminados",
                CuerpoSucursalFecha(fecha), TimeoutIncremental, conIdBloque: true, ct);

            DateTime fechaServidor = LeerFechaServidor(valor);

            if (valor.TryGetProperty("PacientesEliminados", out var lista) && lista.GetArrayLength() > 0)
                await BulkJson.BulkInsertAsync(_db, "pacientes_eliminados", lista, ct: ct);

            await _logInterfaz.GuardaFinLogInterfazAsync(DateTime.Now, "OK", fechaServidor, idLog, ct);

            return "OK|" + Fecha(fechaServidor);
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetPacientesEliminados: " + ex.Message, ex);
        }
    }

    public Task<string> GetMaquinasLaserDetalleAsync(CancellationToken ct = default) =>
        IncrementalPasoAsync("recupera_maquinas_laser_detalle", "carga_inicial_maquinas_laser_detalle",
            "/api/europielpos/GetMaquinasLaserDetalle",
            CuerpoSucursalFecha, "maquinas_laser_detalle", "maquinas_laser_detalle_paso",
            limpiar: c => _db.Database.ExecuteSqlAsync($"EXEC limpia_maquinas_laser_detalle_paso", c),
            procesar: c => _db.Database.ExecuteSqlAsync($"EXEC procesa_maquinas_laser_detalle_paso", c),
            "HttpPost_GetMaquinasLaserDetalle", conIdBloque: false, timeout: TimeoutIncremental, ct: ct);

    /// <remarks>Cada parámetro reemplaza a su versión activa anterior
    /// (port de GuardarValorParametroGeneral de modGeneral.vb).</remarks>
    public async Task<string> GetParametrosGeneralAsync(CancellationToken ct = default)
    {
        try
        {
            DateTime fecha = await _logInterfaz.RecuperaFechaLogInterfazAsync("recupera_parametros_general", "carga_inicial_parametros_general", ct)
                             ?? HoyALas8();

            var valor = await DescargarValorAsync("/api/europielpos/GetParametrosGeneral",
                $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\", \"IdBloque\": \"{_contexto.IdBloque}\", \"Fecha\": \"{Fecha(fecha)}\" }}",
                TimeoutIncremental, conIdBloque: false, ct);

            DateTime fechaServidor = LeerFechaServidor(valor);

            if (valor.TryGetProperty("parametros_general", out var parametros))
            {
                foreach (var p in parametros.EnumerateArray())
                {
                    string? nombre = Texto(p, "parametro_general");
                    string? proveedor = Texto(p, "proveedor");
                    int idSucursal = Entero(p, "id_sucursal");
                    int idBloque = Entero(p, "id_bloque");

                    var anterior = await _db.ParametrosGeneral.FirstOrDefaultAsync(
                        x => x.ParametroGeneral == nombre && x.Proveedor == proveedor
                             && x.IdSucursal == idSucursal && x.IdBloque == idBloque && x.Activo == 1, ct);

                    if (anterior is not null)
                        _db.ParametrosGeneral.Remove(anterior);

                    _db.ParametrosGeneral.Add(new ParametrosGeneral
                    {
                        IdParametroGeneral = Entero(p, "id_parametro_general"),
                        ParametroGeneral = nombre,
                        Proveedor = proveedor,
                        IdSucursal = idSucursal,
                        IdBloque = idBloque,
                        Valor = Texto(p, "valor"),
                        Activo = 1,
                    });

                    await _db.SaveChangesAsync(ct);
                }
            }

            return "OK|" + Fecha(fechaServidor);
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetParametrosGeneral: " + ex.Message, ex);
        }
    }

    public async Task<string> GetPaquetesEliminadosAsync(CancellationToken ct = default)
    {
        try
        {
            DateTime fecha = await _logInterfaz.RecuperaFechaLogInterfazAsync("recupera_paquetes_eliminados", string.Empty, ct)
                             ?? HoyALas8();

            int idLog = await _logInterfaz.GuardaInicioLogInterfazAsync("recupera_paquetes_eliminados", fecha, ct);

            var valor = await DescargarValorAsync("/api/europielpos/GetPaquetesEliminados",
                CuerpoSucursalFecha(fecha), TimeoutIncremental, conIdBloque: false, ct);

            DateTime fechaServidor = LeerFechaServidor(valor);

            if (valor.TryGetProperty("PaquetesEliminados", out var lista) && lista.GetArrayLength() > 0)
                await BulkJson.BulkInsertAsync(_db, "paquetes_eliminados", lista, ct: ct);

            await _logInterfaz.GuardaFinLogInterfazAsync(DateTime.Now, "OK", fechaServidor, idLog, ct);

            return "OK|" + Fecha(fechaServidor);
        }
        catch (Exception ex)
        {
            // (sic) el original reportaba "GetPacientesEliminados" por copy-paste
            throw new Exception("Error HttpPost_GetPaquetesEliminados: " + ex.Message, ex);
        }
    }

    public async Task<(JsonElement Asistidas, JsonElement Borradas)> CitasHistoricoAsync(int claveBloque, int idPaciente, int idSucursal, CancellationToken ct = default)
    {
        try
        {
            string crudo = await _api.PostCrudoAsync("/api/europielpos/GetCitasHistorico",
                $"{{ \"IdPaciente\": \"{idPaciente}\", \"IdSucursal\": \"{idSucursal}\" }}",
                new Dictionary<string, string> { ["ClaveBloque"] = claveBloque.ToString() },
                TimeoutIncremental, ct);

            using var doc = JsonDocument.Parse(crudo);
            var valor = doc.RootElement.GetProperty("Value");

            return (Lista(valor, "Citas_Asistidas"), Lista(valor, "Citas_Borradas"));
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetCitasHistorico: " + ex.Message, ex);
        }
    }

    public async Task<(JsonElement DetallePaquete, JsonElement OtrosPaquetes, JsonElement PlanPagos, JsonElement HistoricoCaja, JsonElement ConsolidadoCaja)>
        CobranzaHistoricoAsync(int claveBloque, int idPaquete, CancellationToken ct = default)
    {
        try
        {
            string crudo = await _api.PostCrudoAsync("/api/europielpos/GetCobranzaHistorico",
                $"{{ \"IdPaquete\": \"{idPaquete}\" }}",
                new Dictionary<string, string> { ["ClaveBloque"] = claveBloque.ToString() },
                TimeoutIncremental, ct);

            using var doc = JsonDocument.Parse(crudo);
            var valor = doc.RootElement.GetProperty("Value");

            return (Lista(valor, "Detalle_Paquete"), Lista(valor, "Otros_Paquetes"), Lista(valor, "Plan_Pagos"),
                    Lista(valor, "Historico_Caja"), Lista(valor, "Consolidado_Caja"));
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_CobranzaHistorico: " + ex.Message, ex);
        }
    }

    public async Task<JsonElement> GetAlertasAsync(int idUsuario, CancellationToken ct = default)
    {
        try
        {
            if (!await _api.HayConexionAsync(ct))
                return ListaVacia();

            var valor = await DescargarValorAsync("/api/europielpos/GetAlertas",
                $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\", \"IdUsuario\": \"{idUsuario}\" }}",
                TimeoutIncremental, conIdBloque: false, ct);

            return Lista(valor, "Alertas");
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetAlertas: " + ex.Message, ex);
        }
    }

    /// <remarks>Único GET del motor; va a EuroAsistente con su propia ApiKey.</remarks>
    public async Task<JsonElement> GetMensajesAsync(string bloque, string usuarioLogin, CancellationToken ct = default)
    {
        try
        {
            if (!await _api.HayConexionAsync(ct))
                return ListaVacia();

            string crudo = await _api.GetCrudoAsync(
                $"/api/euroasistente/GetPendingMessages?bloque={bloque}&login={usuarioLogin}&fuente=POS",
                usarApiKeyEuroAsistente: true, TimeoutIncremental, ct);

            using var doc = JsonDocument.Parse(crudo);

            return doc.RootElement.TryGetProperty("Value", out var valor)
                ? Lista(valor, "Messages")
                : ListaVacia();
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetMensajes: " + ex.Message, ex);
        }
    }

    public async Task<JsonElement> GetIndicadoresUsuarioAsync(int idUsuario, CancellationToken ct = default)
    {
        try
        {
            if (!await _api.HayConexionAsync(ct))
                return ListaVacia();

            var valor = await DescargarValorAsync("/api/europielpos/GetIndicadoresUsuario",
                $"{{ \"IdUsuario\": \"{idUsuario}\" }}", TimeoutIncremental, conIdBloque: false, ct);

            return Lista(valor, "IndicadoresUsuario");
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetIndicadoresUsuario: " + ex.Message, ex);
        }
    }

    // ----- Pipeline incremental común (fecha bitácora → paso → SP → OK|fecha) -----

    private async Task<string> IncrementalPasoAsync(string tipoLog, string tipoCargaInicial, string servicio,
        Func<DateTime, string> cuerpo, string nombreLista, string tablaPaso,
        Func<CancellationToken, Task> limpiar, Func<CancellationToken, Task> procesar,
        string nombreError, bool conIdBloque, int timeout, bool ajustarFechaConMax = false, CancellationToken ct = default)
    {
        try
        {
            DateTime fecha = await _logInterfaz.RecuperaFechaLogInterfazAsync(tipoLog, tipoCargaInicial, ct)
                             ?? HoyALas8();

            var valor = await DescargarValorAsync(servicio, cuerpo(fecha), timeout, conIdBloque, ct);

            DateTime fechaServidor = LeerFechaServidor(valor);

            await limpiar(ct);

            if (valor.TryGetProperty(nombreLista, out var paso) && paso.GetArrayLength() > 0)
            {
                await BulkJson.BulkInsertAsync(_db, tablaPaso, paso, ct: ct);
                await procesar(ct);

                if (ajustarFechaConMax)
                    fechaServidor = AjustaFechaServidor(paso, fecha, fechaServidor);
            }

            return "OK|" + Fecha(fechaServidor);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error {nombreError}: {ex.Message}", ex);
        }
    }

    private string CuerpoSucursalFecha(DateTime fecha) =>
        $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\", \"Fecha\": \"{Fecha(fecha)}\" }}";

    private static DateTime AjustaFechaServidor(JsonElement paso, DateTime fechaSolicitada, DateTime fechaServidor)
    {
        DateTime? fechaConsulta = MaxFecha(paso, "fecha_consulta_interfaz");
        if (fechaConsulta is null)
            return fechaServidor;

        DateTime ajustada = fechaConsulta.Value;
        if (Fecha(fechaSolicitada) == Fecha(ajustada))
            ajustada = ajustada.AddMinutes(1);

        return ajustada;
    }

    private static JsonElement Lista(JsonElement valor, string propiedad) =>
        valor.ValueKind == JsonValueKind.Object && valor.TryGetProperty(propiedad, out var lista) && lista.ValueKind == JsonValueKind.Array
            ? lista.Clone()
            : ListaVacia();

    private static JsonElement ListaVacia()
    {
        using var doc = JsonDocument.Parse("[]");
        return doc.RootElement.Clone();
    }

    private static string? Texto(JsonElement e, string propiedad) =>
        e.TryGetProperty(propiedad, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private static int Entero(JsonElement e, string propiedad) =>
        e.TryGetProperty(propiedad, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetInt32() : 0;
}
