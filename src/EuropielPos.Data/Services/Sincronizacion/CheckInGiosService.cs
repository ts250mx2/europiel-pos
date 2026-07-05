using System.Text.Json;
using EuropielPos.Data.Services;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.Extensions.Options;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Port de la clase <c>CheckInGios</c> de modInterfaz.vb: check-in del
/// personal GIO por reconocimiento facial (servicio Reko). Usa su propia
/// ApiKey (estaba hardcodeada en el original; ahora en configuración).
/// </summary>
public interface ICheckInGiosService
{
    Task<RespuestaCheckinReko> RecuperaPeticionesCheckinGioAsync(string tipo, CancellationToken ct = default);

    Task<RespuestaFromReko> IniciarTomaDeFotosV2Async(string tipo, int idUsuario, CancellationToken ct = default);

    Task<RespuestaFromReko> GetResponseFromRekoAsync(string idFeedback, CancellationToken ct = default);

    Task<string> SaveBranchWithOutUserAsync(string tipo, CancellationToken ct = default);

    Task<string> DeleteCheckinFacialRecognitionAsync(string idFeedback, int idGio, CancellationToken ct = default);
}

public class CheckInGiosService : ICheckInGiosService
{
    private static readonly JsonSerializerOptions Opciones = new() { PropertyNameCaseInsensitive = true };

    private readonly IClienteApiPos _api;
    private readonly ContextoPos _contexto;
    private readonly SincronizacionSettings _settings;
    private readonly IReconocimientoFacialService _reconocimiento;

    public CheckInGiosService(IClienteApiPos api, ContextoPos contexto,
        IOptions<SincronizacionSettings> settings, IReconocimientoFacialService reconocimiento)
    {
        _api = api;
        _contexto = contexto;
        _settings = settings.Value;
        _reconocimiento = reconocimiento;
    }

    /// <summary>BranchId del original: clave de bloque + id de sucursal a 3 dígitos.</summary>
    private string BranchId => _contexto.ClaveBloque + _contexto.IdSucursal.ToString("000");

    public async Task<RespuestaCheckinReko> RecuperaPeticionesCheckinGioAsync(string tipo, CancellationToken ct = default)
    {
        try
        {
            var respuesta = await PostRekoAsync<RespuestaCheckinReko>(
                "/api/facialrecognition/GetRequestCheckinInRekoPOS",
                $"{{ \"BranchId\": \"{BranchId}\", \"Type\": \"{tipo}\" }}", 30, ct);

            // El original solo aceptaba la respuesta cuando Message venía vacío.
            return respuesta is not null && respuesta.Message == string.Empty
                ? respuesta
                : new RespuestaCheckinReko();
        }
        catch (Exception ex)
        {
            throw new Exception("Error RecuperaPeticionesCheckinGio: " + ex.Message, ex);
        }
    }

    /// <summary>Dispara la toma de fotos en la tablet Reko y registra el envío
    /// en la bitácora local antes de llamar al servicio, como el original.</summary>
    public async Task<RespuestaFromReko> IniciarTomaDeFotosV2Async(string tipo, int idUsuario, CancellationToken ct = default)
    {
        string idFeedback = Guid.NewGuid().ToString();
        var respuesta = new RespuestaFromReko { IdFeedback = idFeedback };

        await _reconocimiento.GuardaEnvioReconocimientoFacialAsync(
            Convert.ToInt32(BranchId), tipo, idFeedback, idUsuario, ct);

        try
        {
            var servicios = await PostRekoAsync<RespuestaFromReko>(
                "/api/facialrecognition/SendFacialRecognitionRequestV2",
                $"{{ \"BranchId\": \"{BranchId}\", \"IdFeedback\": \"{idFeedback}\", \"Type\": \"{tipo}\" }}", 60, ct);

            if (servicios is null)
            {
                respuesta.Message = "Error - 001";
                return respuesta;
            }

            if (servicios.Message == string.Empty)
            {
                servicios.IdFeedback = idFeedback;
                return servicios;
            }

            respuesta.Message = servicios.Message;
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Message = ex.Message;
            return respuesta;
        }
    }

    public async Task<RespuestaFromReko> GetResponseFromRekoAsync(string idFeedback, CancellationToken ct = default)
    {
        try
        {
            return await PostRekoAsync<RespuestaFromReko>(
                "/api/facialrecognition/GetResponseReko",
                $"{{ \"IdFeedback\": \"{idFeedback}\" }}", 30, ct) ?? new RespuestaFromReko();
        }
        catch
        {
            return new RespuestaFromReko(); // el original tragaba el error
        }
    }

    public async Task<string> SaveBranchWithOutUserAsync(string tipo, CancellationToken ct = default)
    {
        try
        {
            string idFeedback = Guid.NewGuid().ToString();

            var respuesta = await PostRekoAsync<RespuestaSimple>(
                "/api/facialrecognition/SaveBranchWithOutUser",
                $"{{ \"BranchId\": \"{BranchId}\", \"IdFeedback\": \"{idFeedback}\", \"Type\": \"{tipo}\" }}", 60, ct);

            return respuesta?.Message == string.Empty
                ? "OK"
                : "Error HttpPost_SaveBranchWithOutUser: " + respuesta?.Message;
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }

    public async Task<string> DeleteCheckinFacialRecognitionAsync(string idFeedback, int idGio, CancellationToken ct = default)
    {
        var respuesta = await PostRekoAsync<RespuestaSimple>(
            "/api/facialrecognition/DeleteCheckinFacialRecognition",
            $"{{ \"IdFeedback\": \"{idFeedback}\", \"IdGio\": \"{idGio}\" }}", 60, ct);

        return respuesta?.Message == string.Empty
            ? "OK"
            : "Error HttpPost_DeleteCheckinFacialRecognition: " + respuesta?.Message;
    }

    /// <summary>POST al servicio Reko con la ApiKey de reconocimiento facial.
    /// Se manda por header sobrescribiendo la del cliente general.</summary>
    private async Task<T?> PostRekoAsync<T>(string servicio, string cuerpo, int timeoutSegundos, CancellationToken ct)
    {
        string crudo = await _api.PostCrudoAsync(servicio, cuerpo,
            new Dictionary<string, string>
            {
                ["ClaveBloque"] = _contexto.ClaveBloque,
                ["ApiKey"] = _settings.ApiKeyReconocimientoFacial,
            }, timeoutSegundos, ct);

        return JsonSerializer.Deserialize<T>(crudo, Opciones);
    }
}
