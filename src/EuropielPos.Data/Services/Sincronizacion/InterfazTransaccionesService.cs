using System.Text.Json;
using EuropielPos.Data.Entities;
using EuropielPos.Data.Services;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Bloque 4 del port de <c>modInterfaz.vb</c>: descarga de transacciones
/// (paquetes, pacientes, requerimientos...) — carga inicial masiva a tablas
/// finales e incrementales vía tablas de paso + SP de proceso.
/// Los incrementales devuelven <c>"OK|fecha_servidor"</c> para que el
/// orquestador registre hasta dónde se sincronizó.
/// </summary>
public interface IInterfazTransaccionesService
{
    Task<string> GetPaquetesCargaInicialAsync(CancellationToken ct = default);

    Task<string> GetPaquetesAsync(CancellationToken ct = default);

    Task<string> GetPacientesCargaInicialAsync(CancellationToken ct = default);

    Task<string> GetRequerimientosAsync(CancellationToken ct = default);

    Task<string> GetTipoFallaAsync(CancellationToken ct = default);
}

public partial class InterfazTransaccionesService : IInterfazTransaccionesService
{
    private const int TimeoutCargaInicial = 300; // 5 min, como el original
    private const int TimeoutIncremental = 120;

    private readonly PosDbContext _db;
    private readonly IClienteApiPos _api;
    private readonly ContextoPos _contexto;
    private readonly ILogInterfazService _logInterfaz;
    private readonly IPaqueteService _paquete;
    private readonly IRequerimientoService _requerimiento;

    public InterfazTransaccionesService(PosDbContext db, IClienteApiPos api, ContextoPos contexto,
        ILogInterfazService logInterfaz, IPaqueteService paquete, IRequerimientoService requerimiento)
    {
        _db = db;
        _api = api;
        _contexto = contexto;
        _logInterfaz = logInterfaz;
        _paquete = paquete;
        _requerimiento = requerimiento;
    }

    public async Task<string> GetPaquetesCargaInicialAsync(CancellationToken ct = default)
    {
        try
        {
            var valor = await DescargarValorAsync("/api/europielpos/GetPaquetesCA", string.Empty,
                TimeoutCargaInicial, conIdBloque: true, ct);

            if (valor.TryGetProperty("Paquetes", out var paquetes))
            {
                await BulkJson.BulkInsertAsync(_db, "paquete", paquetes,
                    new Dictionary<string, object?> { ["fecha_interfaz"] = DateTime.Now }, ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetPaquetesCargaInicial: " + ex.Message, ex);
        }
    }

    /// <remarks>Incremental GetPaquetesV2: pide desde la última fecha de la
    /// bitácora, llena paquete_paso y lo procesa. Devuelve la fecha del
    /// servidor ajustada (max fecha_consulta_interfaz, +1 min si no avanzó).</remarks>
    public async Task<string> GetPaquetesAsync(CancellationToken ct = default)
    {
        try
        {
            DateTime fecha = await _logInterfaz.RecuperaFechaLogInterfazAsync("recupera_paquete", "carga_inicial_paquete", ct)
                             ?? HoyALas8();

            var valor = await DescargarValorAsync("/api/europielpos/GetPaquetesV2",
                $"{{ \"Fecha\": \"{Fecha(fecha)}\" }}", 50, conIdBloque: true, ct);

            DateTime fechaServidor = LeerFechaServidor(valor);

            await _paquete.LimpiarPaquetePasoAsync(ct);

            if (valor.TryGetProperty("PaquetePaso", out var paso) && paso.GetArrayLength() > 0)
            {
                await BulkJson.BulkInsertAsync(_db, "paquete_paso", paso, ct: ct);
                await _paquete.ProcesaPaquetePasoAsync(ct);

                DateTime? fechaConsulta = MaxFecha(paso, "fecha_consulta_interfaz");
                if (fechaConsulta is not null)
                {
                    fechaServidor = fechaConsulta.Value;

                    if (Fecha(fecha) == Fecha(fechaServidor))
                        fechaServidor = fechaServidor.AddMinutes(1);
                }
            }

            return "OK|" + Fecha(fechaServidor);
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetPaquetes: " + ex.Message, ex);
        }
    }

    public async Task<string> GetPacientesCargaInicialAsync(CancellationToken ct = default)
    {
        try
        {
            var valor = await DescargarValorAsync("/api/europielpos/GetPacientesCA",
                $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\" }}", TimeoutCargaInicial, conIdBloque: true, ct);

            if (valor.TryGetProperty("Pacientes", out var pacientes))
            {
                await BulkJson.BulkInsertAsync(_db, "paciente", pacientes,
                    new Dictionary<string, object?> { ["fecha_interfaz"] = DateTime.Now }, ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetPacientesCargaInicial: " + ex.Message, ex);
        }
    }

    public async Task<string> GetRequerimientosAsync(CancellationToken ct = default)
    {
        try
        {
            DateTime fecha = await _logInterfaz.RecuperaFechaLogInterfazAsync("recupera_requerimiento", "carga_inicial_requerimientos", ct)
                             ?? HoyALas8();

            var valor = await DescargarValorAsync("/api/europielpos/GetRequerimientos",
                $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\", \"Fecha\": \"{Fecha(fecha)}\" }}",
                TimeoutIncremental, conIdBloque: false, ct);

            DateTime fechaServidor = LeerFechaServidor(valor);

            await _requerimiento.LimpiarRequerimientoPasoAsync(ct);

            if (valor.TryGetProperty("Requerimientos", out var paso) && paso.GetArrayLength() > 0)
            {
                await BulkJson.BulkInsertAsync(_db, "requerimiento_paso", paso, ct: ct);
                await _requerimiento.ProcesaRequerimientoPasoAsync(ct);
            }

            return "OK|" + Fecha(fechaServidor);
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetRequerimientos: " + ex.Message, ex);
        }
    }

    /// <remarks>Reemplazo total del catálogo de tipos de falla. OJO: devuelve
    /// <c>OK|0001-01-01...</c> porque el original nunca asignaba fecha_servidor
    /// en este método (el orquestador no la usa aquí).</remarks>
    public async Task<string> GetTipoFallaAsync(CancellationToken ct = default)
    {
        try
        {
            DateTime fecha = await _logInterfaz.RecuperaFechaLogInterfazAsync("recupera_tipo_falla", "carga_inicial_tipo_falla", ct)
                             ?? HoyALas8();

            var valor = await DescargarValorAsync("/api/europielpos/GetTipo_Falla",
                $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\", \"Fecha\": \"{Fecha(fecha)}\" }}",
                TimeoutIncremental, conIdBloque: false, ct);

            if (valor.TryGetProperty("TipoFalla", out var tipos) && tipos.GetArrayLength() > 0)
            {
                await _db.TipoFalla.Where(x => x.IdTipoFalla > 0).ExecuteDeleteAsync(ct);

                foreach (var t in tipos.EnumerateArray())
                {
                    _db.TipoFalla.Add(new TipoFalla
                    {
                        IdTipoFalla = t.GetProperty("id_tipo_falla").GetInt32(),
                        Descripcion = t.TryGetProperty("descripcion", out var d) ? d.GetString() : null,
                        TipoRequerimiento = t.TryGetProperty("tipo_requerimiento", out var tr) ? tr.GetString() : null,
                    });
                }

                await _db.SaveChangesAsync(ct);
            }

            return "OK|" + Fecha(default(DateTime)); // bug histórico replicado
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetTipo_Falla: " + ex.Message, ex);
        }
    }

    // ----- Pipeline común -----

    /// <summary>POST al API y devuelve el elemento <c>Value</c> de la respuesta.</summary>
    private async Task<JsonElement> DescargarValorAsync(string servicio, string cuerpo, int timeoutSegundos, bool conIdBloque, CancellationToken ct)
    {
        var headers = new Dictionary<string, string> { ["ClaveBloque"] = _contexto.ClaveBloque };
        if (conIdBloque)
            headers["IdBloque"] = _contexto.IdBloque.ToString();

        string crudo = await _api.PostCrudoAsync(servicio, cuerpo, headers, timeoutSegundos, ct);

        using var doc = JsonDocument.Parse(crudo);

        return doc.RootElement.TryGetProperty("Value", out var valor) && valor.ValueKind == JsonValueKind.Object
            ? valor.Clone()
            : default;
    }

    private static DateTime LeerFechaServidor(JsonElement valor) =>
        valor.ValueKind == JsonValueKind.Object
        && valor.TryGetProperty("fecha_servidor", out var f)
        && f.ValueKind == JsonValueKind.String
        && f.TryGetDateTime(out var fecha)
            ? fecha
            : default;

    private static DateTime? MaxFecha(JsonElement lista, string propiedad)
    {
        DateTime? max = null;

        foreach (var fila in lista.EnumerateArray())
        {
            if (fila.TryGetProperty(propiedad, out var v) && v.ValueKind == JsonValueKind.String && v.TryGetDateTime(out var fecha))
            {
                if (max is null || fecha > max)
                    max = fecha;
            }
        }

        return max;
    }

    private static DateTime HoyALas8() =>
        new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);

    private static string Fecha(DateTime fecha) => fecha.ToString("yyyy-MM-dd HH:mm:ss");
}
