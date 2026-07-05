using System.Text;
using System.Text.Json;
using EuropielPos.Data.Entities;
using EuropielPos.Data.Services;
using EuropielPos.Domain.Diagnostico;
using EuropielPos.Domain.Pagos;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Port de <c>modFeenicia.vb</c>: suscripciones del procesador Feenicia/Serti
/// y su sincronización con el servidor central, incluido el orquestador
/// ProcesoInterfazFeenicia.
/// </summary>
public interface IFeeniciaService
{
    /// <summary>Alta de suscripción directa en el API de Serti (el JSON y el
    /// header X-Requested-With cifrado los arma el llamador, como el original).</summary>
    Task<RespuestaSuscribeFeenicia> AddSuscribeAsync(string jsonData, string encriptado, CancellationToken ct = default);

    /// <summary>Cobro en la terminal MPOS local de Feenicia.</summary>
    Task<MposResponse?> SendDataMposAsync(string jsonData, string feecontrollerUrl, CancellationToken ct = default);

    Task<string> SaveFeeniciaSuscripcionesAsync(FeeniciaSubscripciones p, CancellationToken ct = default);

    Task<List<FeeniciaSubscripciones>> RecuperaFeeniciaSuscripcionesEnviarAsync(CancellationToken ct = default);

    Task ActualizaFechaInterfazFeeniciaSuscripcionesAsync(int idLocal, DateTime fecha, CancellationToken ct = default);

    /// <summary>Port de ProcesoInterfazFeenicia: envía las suscripciones pendientes.</summary>
    Task ProcesoInterfazFeeniciaAsync(Action<string>? reportaEstado = null, CancellationToken ct = default);
}

public class FeeniciaService : IFeeniciaService
{
    private static readonly JsonSerializerOptions Opciones = new() { PropertyNameCaseInsensitive = true };

    private readonly PosDbContext _db;
    private readonly IClienteApiPos _api;
    private readonly ContextoPos _contexto;
    private readonly ILogInterfazService _log;
    private readonly HttpClient _http;
    private readonly SincronizacionSettings _settings;
    private readonly EscritorLog _escritorLog = new();

    public FeeniciaService(PosDbContext db, IClienteApiPos api, ContextoPos contexto,
        ILogInterfazService log, HttpClient http, IOptions<SincronizacionSettings> settings)
    {
        _db = db;
        _api = api;
        _contexto = contexto;
        _log = log;
        _http = http;
        _settings = settings.Value;
    }

    public async Task<RespuestaSuscribeFeenicia> AddSuscribeAsync(string jsonData, string encriptado, CancellationToken ct = default)
    {
        var mensajes = new StringBuilder();
        mensajes.AppendLine($"[{DateTime.Now}] \t--ENVIANDO DATOS PARA SUBSCRIPCIÓN--");

        try
        {
            using var peticion = new HttpRequestMessage(HttpMethod.Post, _settings.UrlFeeniciaSuscribe)
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json"),
            };
            peticion.Headers.Add("Cache-Control", "no-cache");
            peticion.Headers.Add("X-Requested-With", encriptado);

            using var respuestaHttp = await _http.SendAsync(peticion, ct);
            string texto = await respuestaHttp.Content.ReadAsStringAsync(ct);
            mensajes.AppendLine($"[{DateTime.Now}] \t{texto}");

            var resultado = JsonSerializer.Deserialize<RespuestaSuscribeFeenicia>(texto, Opciones)
                            ?? new RespuestaSuscribeFeenicia();

            if (resultado.responseCode != "00")
                mensajes.AppendLine($"[{DateTime.Now}] \t{resultado.responseMessage}");

            _escritorLog.EscribirEnLog(mensajes.ToString(), "SuscribeFeenicia");

            return resultado;
        }
        catch (Exception ex)
        {
            _escritorLog.EscribirEnLog(ex.Message, "SuscribeFeenicia");
            throw new Exception("Error AddSuscribe: " + ex.Message, ex);
        }
    }

    public async Task<MposResponse?> SendDataMposAsync(string jsonData, string feecontrollerUrl, CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(90)); // timeout del original

            using var peticion = new HttpRequestMessage(HttpMethod.Post, feecontrollerUrl)
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json"),
            };
            peticion.Headers.Add("Cache-Control", "no-cache");

            using var respuesta = await _http.SendAsync(peticion, cts.Token);
            string texto = await respuesta.Content.ReadAsStringAsync(cts.Token);

            return JsonSerializer.Deserialize<MposResponse>(texto, Opciones);
        }
        catch (Exception ex)
        {
            throw new Exception("Error sendDataMpos: " + ex.Message, ex);
        }
    }

    public async Task<string> SaveFeeniciaSuscripcionesAsync(FeeniciaSubscripciones p, CancellationToken ct = default)
    {
        try
        {
            var dto = new FeeniciaSuscripcionSimple
            {
                id_local = p.IdLocal,
                fecha_modificacion_local = p.FechaModificacionLocal,
                fecha_interfaz = p.FechaInterfaz,
                id_subscripcion = p.IdSubscripcion,
                mensaje_respuesta = p.MensajeRespuesta,
                codigo_respuesta = p.CodigoRespuesta,
                id_plan = p.IdPlan,
                plan_unit = p.PlanUnit,
                payload = p.Payload,
                id_local_paquete = p.IdLocalPaquete,
                id_paquete = p.IdPaquete,
                id_sucursal = p.IdSucursal,
                id_bloque = p.IdBloque,
                activo = p.Activo,
            };

            string crudo = await _api.PostCrudoAsync("/api/europielpos/SaveFeenicia_Suscripciones",
                JsonSerializer.Serialize(dto),
                new Dictionary<string, string> { ["ClaveBloque"] = _contexto.ClaveBloque }, ct: ct);

            var respuesta = JsonSerializer.Deserialize<RespuestaSimple>(crudo, Opciones);

            return respuesta?.Message == "OK"
                ? "OK"
                : "Error HttpPost_SaveFeenicia_Subcripciones: " + crudo; // (sic) typo del original
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SaveFeenicia_Subcripciones: " + ex.Message;
        }
    }

    /// <remarks>El SP solo existe en las BDs de tenants con Feenicia habilitado.</remarks>
    public Task<List<FeeniciaSubscripciones>> RecuperaFeeniciaSuscripcionesEnviarAsync(CancellationToken ct = default) =>
        ProcedimientoAlmacenado.ListaAsync(_db, "recupera_feenicia_suscripciones_enviar",
            new Dictionary<string, object?> { ["@fecha_hoy"] = DateTime.Today },
            r => new FeeniciaSubscripciones
            {
                IdLocal = Convert.ToInt32(r["id_local"]),
                FechaModificacionLocal = r.FechaONull("fecha_modificacion_local"),
                FechaInterfaz = r.FechaONull("fecha_interfaz"),
                IdSubscripcion = Convert.ToInt32(r["id_subscripcion"]),
                MensajeRespuesta = r.CadenaODefecto("mensaje_respuesta"),
                CodigoRespuesta = r.CadenaODefecto("codigo_respuesta"),
                IdPlan = r.CadenaODefecto("id_plan"),
                PlanUnit = Convert.ToInt32(r["plan_unit"]),
                IdLocalPaquete = Convert.ToInt32(r["id_local_paquete"]),
                IdPaquete = Convert.ToInt32(r["id_paquete"]),
                IdSucursal = Convert.ToInt32(r["id_sucursal"]),
                IdBloque = Convert.ToInt32(r["id_bloque"]),
                Activo = Convert.ToBoolean(r["activo"]),
            }, ct);

    public Task ActualizaFechaInterfazFeeniciaSuscripcionesAsync(int idLocal, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_feenicia_suscripciones @id_local = {idLocal}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss")}", ct);

    public async Task ProcesoInterfazFeeniciaAsync(Action<string>? reportaEstado = null, CancellationToken ct = default)
    {
        try
        {
            if (!await _api.HayConexionAsync(ct))
            {
                reportaEstado?.Invoke($"Estatus: No hay conexión con el servidor, Ultimo intento:  {DateTime.Now:dd/MM/yyyy HH:mm}");
                return;
            }

            var pendientes = await _db.FeeniciaSubscripciones
                .AsNoTracking()
                .Where(x => x.FechaInterfaz == null)
                .ToListAsync(ct);

            var errores = new StringBuilder();
            int idLog = await _log.GuardaInicioLogInterfazAsync("envio_feenicia_suscripciones", DateTime.Now, ct);
            int porEnviar = pendientes.Count;

            foreach (var p in pendientes)
            {
                string resultado = await SaveFeeniciaSuscripcionesAsync(p, ct);

                if (resultado == "OK")
                    await ActualizaFechaInterfazFeeniciaSuscripcionesAsync(p.IdLocal, DateTime.Now, ct);
                else
                    errores.Append(resultado).Append(", ");

                porEnviar--;
                reportaEstado?.Invoke($"Estatus: Conectado, Registros por enviar: {porEnviar}, Última conexión:  {DateTime.Now:dd/MM/yyyy HH:mm}");
            }

            await _log.GuardaFinLogInterfazAsync(DateTime.Now, errores.Length == 0 ? "OK" : errores.ToString(), null, idLog, ct);
        }
        catch
        {
            // el original tragaba cualquier error del proceso completo
        }
    }
}
