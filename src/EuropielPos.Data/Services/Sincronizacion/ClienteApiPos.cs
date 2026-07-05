using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Configuración del API central (sección <c>Sincronizacion</c> de appsettings).
/// La ApiKey vive en <c>appsettings.Local.json</c> — el original la tenía
/// hardcodeada en modGeneral.vb.
/// </summary>
public class SincronizacionSettings
{
    public string UrlApi { get; set; } = "http://app.europiel.com.mx";

    public string ApiKey { get; set; } = string.Empty;

    /// <summary>ApiKey del servicio EuroAsistente (mensajería); estaba
    /// hardcodeada en modGeneral.vb.</summary>
    public string ApiKeyEuroAsistente { get; set; } = string.Empty;

    /// <summary>Timeout por petición; el original usaba 20 segundos.</summary>
    public int TimeoutSegundos { get; set; } = 20;
}

/// <summary>
/// Cliente HTTP del API central. Replica el contrato de modInterfaz.vb:
/// POST a <c>{UrlApi}/api/europielpos/{metodo}</c> con headers <c>ApiKey</c> y
/// <c>ClaveBloque</c> (y opcionales), cuerpo JSON y respuesta JSON.
/// </summary>
public interface IClienteApiPos
{
    /// <summary>POST que deserializa la respuesta a <typeparamref name="T"/>.
    /// <paramref name="timeoutSegundos"/> permite sobrescribir el timeout por
    /// petición (las citas y alertas usaban 90s en el original).</summary>
    Task<T?> PostAsync<T>(string servicio, string cuerpoJson = "", IReadOnlyDictionary<string, string>? headersExtra = null, int? timeoutSegundos = null, CancellationToken ct = default);

    /// <summary>POST que devuelve el cuerpo crudo de la respuesta.</summary>
    Task<string> PostCrudoAsync(string servicio, string cuerpoJson = "", IReadOnlyDictionary<string, string>? headersExtra = null, int? timeoutSegundos = null, CancellationToken ct = default);

    /// <summary>GET crudo (lo usa GetPendingMessages de EuroAsistente, que
    /// viaja con su propia ApiKey).</summary>
    Task<string> GetCrudoAsync(string servicioConQuery, bool usarApiKeyEuroAsistente = false, int? timeoutSegundos = null, CancellationToken ct = default);

    /// <summary>Port de <c>CheckForInternetConnection</c>: ¿responde el API?</summary>
    Task<bool> HayConexionAsync(CancellationToken ct = default);
}

public class ClienteApiPos : IClienteApiPos
{
    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _http;
    private readonly SincronizacionSettings _settings;

    public ClienteApiPos(HttpClient http, IOptions<SincronizacionSettings> settings)
    {
        _settings = settings.Value;
        _http = http;
        // El timeout real se aplica por petición (permite los 90s de citas).
        _http.Timeout = Timeout.InfiniteTimeSpan;
    }

    public async Task<T?> PostAsync<T>(string servicio, string cuerpoJson = "", IReadOnlyDictionary<string, string>? headersExtra = null, int? timeoutSegundos = null, CancellationToken ct = default)
    {
        string respuesta = await PostCrudoAsync(servicio, cuerpoJson, headersExtra, timeoutSegundos, ct);

        return JsonSerializer.Deserialize<T>(respuesta, OpcionesJson);
    }

    public async Task<string> PostCrudoAsync(string servicio, string cuerpoJson = "", IReadOnlyDictionary<string, string>? headersExtra = null, int? timeoutSegundos = null, CancellationToken ct = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(timeoutSegundos ?? _settings.TimeoutSegundos));
        ct = cts.Token;

        using var peticion = new HttpRequestMessage(HttpMethod.Post, _settings.UrlApi + servicio)
        {
            Content = new StringContent(cuerpoJson, Encoding.UTF8, "application/json"),
        };

        peticion.Headers.Add("ApiKey", _settings.ApiKey);

        // ClaveBloque siempre viaja; el valor por defecto del original era "1".
        if (headersExtra is null || !headersExtra.ContainsKey("ClaveBloque"))
            peticion.Headers.Add("ClaveBloque", "1");

        if (headersExtra is not null)
        {
            foreach (var (clave, valor) in headersExtra)
                peticion.Headers.Add(clave, valor);
        }

        using var respuesta = await _http.SendAsync(peticion, ct);
        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadAsStringAsync(ct);
    }

    public async Task<string> GetCrudoAsync(string servicioConQuery, bool usarApiKeyEuroAsistente = false, int? timeoutSegundos = null, CancellationToken ct = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(timeoutSegundos ?? _settings.TimeoutSegundos));

        using var peticion = new HttpRequestMessage(HttpMethod.Get, _settings.UrlApi + servicioConQuery);
        peticion.Headers.Add("ApiKey", usarApiKeyEuroAsistente ? _settings.ApiKeyEuroAsistente : _settings.ApiKey);

        using var respuesta = await _http.SendAsync(peticion, cts.Token);
        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadAsStringAsync(cts.Token);
    }

    public async Task<bool> HayConexionAsync(CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(_settings.TimeoutSegundos));

            using var respuesta = await _http.GetAsync(_settings.UrlApi, cts.Token);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
