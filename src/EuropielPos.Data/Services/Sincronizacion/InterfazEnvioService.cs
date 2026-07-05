using System.Text;
using EuropielPos.Data.Entities;
using EuropielPos.Data.Services;
using EuropielPos.Domain.Sincronizacion;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Bloque 3 del port de <c>modInterfaz.vb</c>: envío de transacciones locales
/// al servidor central (los <c>HttpPost_Save*</c>). Todos devuelven "OK" o el
/// mensaje de error como texto — nunca lanzan excepción, igual que el
/// original (el orquestador decide con base en el texto).
/// </summary>
public interface IInterfazEnvioService
{
    Task<string> SaveGioAsync(int idGioLocal, CancellationToken ct = default);

    Task<string> SaveGioAsistenciaAsync(int idGioAsistenciaLocal, CancellationToken ct = default);

    Task<string> SaveRequerimientoAsync(int idRequerimientoLocal, CancellationToken ct = default);

    Task<string> SavePacienteAsync(int idPacienteLocal, CancellationToken ct = default);

    Task<string> SavePaqueteAsync(Paquete paquete, CancellationToken ct = default);

    Task<string> SavePagoAsync(PagoCaja pagoCaja, CancellationToken ct = default);

    Task<string> SaveCitaAsync(Cita cita, CancellationToken ct = default);

    Task<string> CancelCitaAsync(int idCitaLocal, int idCita, int idUsuario, CancellationToken ct = default);

    Task<string> SavePaqueteReimpresionContratoAsync(int idPaquete, int idCita, CancellationToken ct = default);

    Task<string> SaveAlertaResponseAsync(int idCita, string respuesta, int idUsuario, CancellationToken ct = default);

    Task<string> SaveRespuestaNetpayAsync(RespuestaNetpay r, CancellationToken ct = default);

    Task<string> SaveDocumentoAsync(int idTipoDocumento, int idRelacion, int idUsuario, string nombre,
        string referencia, int idSucursal, int noCajaInterfaz, int idRemotoInterfaz, CancellationToken ct = default);

    Task<string> SaveDocumentoAwsAsync(int idTipoDocumento, int idRelacion, int idUsuario, string nombre,
        string referencia, int idSucursal, int noCajaInterfaz, int idRemotoInterfaz,
        string nombreAws, string urlAws, CancellationToken ct = default);
}

public class InterfazEnvioService : IInterfazEnvioService
{
    private const int TimeoutCitas = 90; // el original usaba 1.5 min para citas/alertas

    private readonly IClienteApiPos _api;
    private readonly ContextoPos _contexto;
    private readonly IGioService _gio;
    private readonly IRequerimientoService _requerimiento;
    private readonly IPaqueteEnvioService _paqueteEnvio;
    private readonly IPagoCajaService _pagoCaja;
    private readonly ICitaService _cita;

    public InterfazEnvioService(IClienteApiPos api, ContextoPos contexto, IGioService gio,
        IRequerimientoService requerimiento, IPaqueteEnvioService paqueteEnvio,
        IPagoCajaService pagoCaja, ICitaService cita)
    {
        _api = api;
        _contexto = contexto;
        _gio = gio;
        _requerimiento = requerimiento;
        _paqueteEnvio = paqueteEnvio;
        _pagoCaja = pagoCaja;
        _cita = cita;
    }

    public async Task<string> SaveGioAsync(int idGioLocal, CancellationToken ct = default)
    {
        try
        {
            string json = await _gio.RecuperaJsonGioAEnviarAsync(idGioLocal, _contexto.Bloque, _contexto.NoCaja ?? 0, _contexto.IdSucursal, ct);
            return await EnviarAsync("/api/europielpos/SaveGIO", json, "HttpPost_SaveGIO", ct: ct);
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SaveGIO: " + ex.Message;
        }
    }

    public async Task<string> SaveGioAsistenciaAsync(int idGioAsistenciaLocal, CancellationToken ct = default)
    {
        try
        {
            string json = await _gio.RecuperaJsonGioAsistenciaAEnviarAsync(idGioAsistenciaLocal, _contexto.Bloque, _contexto.NoCaja ?? 0, _contexto.IdSucursal, ct);
            return await EnviarAsync("/api/europielpos/SaveGIO_Asistencia", json, "HttpPost_SaveGIO_Asistencia", ct: ct);
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SaveGIO_Asistencia: " + ex.Message;
        }
    }

    public async Task<string> SaveRequerimientoAsync(int idRequerimientoLocal, CancellationToken ct = default)
    {
        try
        {
            string json = await _requerimiento.RecuperaJsonRequerimientoAEnviarAsync(idRequerimientoLocal, _contexto.Bloque, _contexto.NoCaja ?? 0, _contexto.IdSucursal, ct);
            return await EnviarAsync("/api/europielpos/SaveRequerimiento", json, "HttpPost_SaveRequerimiento", ct: ct);
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SaveRequerimiento: " + ex.Message;
        }
    }

    public async Task<string> SavePacienteAsync(int idPacienteLocal, CancellationToken ct = default)
    {
        try
        {
            string json = await _paqueteEnvio.RecuperaJsonPacienteAEnviarAsync(idPacienteLocal, _contexto.Bloque, _contexto.NoCaja ?? 0, ct);
            return await EnviarAsync("/api/europielpos/SavePaciente", json, "HttpPost_SavePaciente", ct: ct);
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SavePaciente: " + ex.Message;
        }
    }

    public async Task<string> SavePaqueteAsync(Paquete paquete, CancellationToken ct = default)
    {
        try
        {
            string json = await _paqueteEnvio.RecuperaJsonPaqueteAEnviarAsync(paquete, _contexto.Bloque, _contexto.NoCaja ?? 0, _contexto.IdSucursal, ct);
            return await EnviarAsync("/api/europielpos/SavePaquete", json, "HttpPost_SavePaquete", ct: ct);
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SavePaquete: " + ex.Message;
        }
    }

    /// <remarks>OJO: el payload del pago se envía tal cual lo arma
    /// RecuperaJsonPagoCajaAEnviar — SIN la llave de cierre exterior, porque
    /// así lo mandaba el original y así lo tolera el servidor.</remarks>
    public async Task<string> SavePagoAsync(PagoCaja pagoCaja, CancellationToken ct = default)
    {
        try
        {
            string json = await _pagoCaja.RecuperaJsonPagoCajaAEnviarAsync(pagoCaja, _contexto.Bloque, _contexto.NoCaja ?? 0, _contexto.IdSucursal, ct);
            return await EnviarAsync("/api/europielpos/SavePagoCaja", json, "HttpPost_SavePago", ct: ct);
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SavePago: " + ex.Message;
        }
    }

    public async Task<string> SaveCitaAsync(Cita cita, CancellationToken ct = default)
    {
        try
        {
            cita.FechaFin = await _cita.RecuperaFechaFinParaEnvioCitaAsync(cita.IdLocal, ct);
            cita.EsConsultaMedica ??= false;

            var json = new StringBuilder();
            json.Append(
                $"{{\"g_cita\": {{\"bloque\": \"{_contexto.Bloque}\", \"id_sucursal\": \"{_contexto.IdSucursal}\", " +
                $"\"no_caja\": \"{_contexto.NoCaja}\", \"id_local\": \"{cita.IdLocal}\", " +
                $"\"fecha_modificacion_local\": \"{Fecha(cita.FechaModificacionLocal)}\", " +
                $"\"fecha_interfaz\": \"{Fecha(cita.FechaInterfaz)}\", \"id_cita\": \"{cita.IdCita}\", " +
                $"\"fecha_inicio\": \"{Fecha(cita.FechaInicio)}\", \"fecha_fin\": \"{Fecha(cita.FechaFin)}\", " +
                $"\"id_paciente\": \"{cita.IdPaciente}\", \"id_paciente_local\": \"{cita.IdPacienteLocal}\", " +
                $"\"usuario_alta\": \"{cita.UsuarioAlta}\", \"id_servicio\": \"{cita.IdServicio}\", " +
                $"\"id_paquete\": \"{cita.IdPaquete}\", \"id_paquete_local\": \"{cita.IdPaqueteLocal}\", " +
                $"\"fecha_alta\": \"{Fecha(cita.FechaAlta)}\", \"es_consulta_medica\": \"{cita.EsConsultaMedica}\" }}");

            var servicios = await _cita.RecuperaCitaServiciosAsync(cita.IdLocal, ct);

            if (servicios.Count > 0)
            {
                json.Append(",\"g_cita_servicio\":[");
                var detalle = new StringBuilder();

                foreach (var s in servicios)
                {
                    detalle.Append(
                        $"{{\"bloque\": \"{_contexto.Bloque}\", \"id_sucursal\": \"{_contexto.IdSucursal}\", " +
                        $"\"no_caja\": \"{_contexto.NoCaja}\", " +
                        $"\"fecha_modificacion_local\": \"{Fecha(s.FechaModificacionLocal)}\", " +
                        $"\"fecha_interfaz\": \"{Fecha(s.FechaInterfaz)}\", \"id_local\": \"{s.IdLocal}\", " +
                        $"\"id_detalle\": \"{s.IdDetalle}\", \"id_cita_local\": \"{s.IdCitaLocal}\", " +
                        $"\"id_cita\": \"{s.IdCita}\", \"id_servicio\": \"{s.IdServicio}\", " +
                        $"\"id_paquete\": \"{s.IdPaquete}\", \"id_paquete_local\": \"{s.IdPaqueteLocal}\", " +
                        $"\"tipo\": \"{s.Tipo}\" }},");
                }

                if (detalle.Length > 0)
                    detalle.Length--;

                json.Append(detalle).Append(']');
            }
            else
            {
                json.Append(",\"g_cita_servicio\":null");
            }

            json.Append("}}");

            return await EnviarAsync("/api/europielpos/SaveCita", json.ToString(), "HttpPost_SaveCita",
                comparaConStartsWith: true, timeoutSegundos: TimeoutCitas, ct: ct);
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SaveCita: " + ex.Message;
        }
    }

    public async Task<string> CancelCitaAsync(int idCitaLocal, int idCita, int idUsuario, CancellationToken ct = default)
    {
        try
        {
            string json =
                $"{{\"g_cita_a_borrar\": {{\"bloque\": \"{_contexto.Bloque}\", \"id_sucursal\": \"{_contexto.IdSucursal}\", " +
                $"\"no_caja\": \"{_contexto.NoCaja}\", \"id_local\": \"{idCitaLocal}\", " +
                $"\"id_cita\": \"{idCita}\", \"id_usuario\": \"{idUsuario}\" }}}}";

            return await EnviarAsync("/api/europielpos/CancelCita", json, prefijoError: null,
                comparaConStartsWith: true, timeoutSegundos: TimeoutCitas, ct: ct);
        }
        catch (Exception ex)
        {
            return ex.Message; // el original devolvía el mensaje pelón
        }
    }

    public async Task<string> SavePaqueteReimpresionContratoAsync(int idPaquete, int idCita, CancellationToken ct = default)
    {
        try
        {
            string json = $"{{\"paquete_reimpresion_contrato\": {{\"id_paquete\": \"{idPaquete}\", \"id_cita\": \"{idCita}\"}}}}";

            return await EnviarAsync("/api/europielpos/SavePaqueteReimpresionContrato", json, prefijoError: null,
                comparaConStartsWith: true, timeoutSegundos: TimeoutCitas, ct: ct);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string> SaveAlertaResponseAsync(int idCita, string respuesta, int idUsuario, CancellationToken ct = default)
    {
        try
        {
            string json = $"{{\"g_alerta_respuesta\": {{\"id_cita\": \"{idCita}\", \"respuesta\": \"{respuesta}\", \"id_usuario\": \"{idUsuario}\" }}}}";

            return await EnviarAsync("/api/europielpos/SaveAlertaResponse", json, prefijoError: null,
                comparaConStartsWith: true, timeoutSegundos: TimeoutCitas, ct: ct);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string> SaveRespuestaNetpayAsync(RespuestaNetpay r, CancellationToken ct = default)
    {
        try
        {
            string json =
                $"{{\"respuesta_netpay\": {{\"bloque\": \"{_contexto.Bloque}\", \"id_sucursal\": \"{_contexto.IdSucursal}\", " +
                $"\"no_caja\": \"{_contexto.NoCaja}\", \"id_local\": \"{r.Id}\", " +
                $"\"fecha\": \"{Fecha(r.Fecha)}\", \"OrderId\": \"{r.OrderId}\", \"TransactionId\": \"{r.TransactionId}\", " +
                $"\"CardNumber\": \"{r.CardNumber}\", \"ResponseCode\": \"{r.ResponseCode}\", \"ResponseMsg\": \"{r.ResponseMsg}\", " +
                $"\"ResponseText\": \"{r.ResponseText}\", \"ARQC\": \"{r.Arqc}\", \"CVM\": \"{r.Cvm}\", " +
                $"\"AID\": \"{r.Aid}\", \"MerchantId\": \"{r.MerchantId}\", \"AuthCode\": \"{r.AuthCode}\", " +
                $"\"CustomerName\": \"{r.CustomerName}\", \"BankName\": \"{r.BankName}\", \"CardTypeName\": \"{r.CardTypeName}\", " +
                $"\"CardType\": \"{r.CardType}\", \"POSEntryMode\": \"{r.PosentryMode}\", \"CardToken\": \"{r.CardToken}\", " +
                $"\"monto\": \"{r.Monto}\", \"id_paciente\": \"{r.IdPaciente}\", \"paciente\": \"{r.Paciente}\", " +
                $"\"promocion\": \"{r.Promocion}\", \"tipo\": \"{r.Tipo}\", \"procesador\": \"{r.Procesador}\"}}}}";

            return await EnviarAsync("/api/europielpos/SaveRespuestaNetpay", json, "HttpPost_SaveRespuestaNetpay", ct: ct);
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SaveRespuestaNetpay: " + ex.Message;
        }
    }

    public Task<string> SaveDocumentoAsync(int idTipoDocumento, int idRelacion, int idUsuario, string nombre,
        string referencia, int idSucursal, int noCajaInterfaz, int idRemotoInterfaz, CancellationToken ct = default)
    {
        string json =
            $"{{\"documento\": {{\"id_tipo_documento\": \"{idTipoDocumento}\", \"id_relacion\": \"{idRelacion}\", " +
            $"\"id_usuario_alta\": \"{idUsuario}\", \"nombre\": \"{nombre}\", \"referencia\": \"{referencia}\", " +
            $"\"id_sucursal\": \"{idSucursal}\", \"no_caja_interfaz\": \"{noCajaInterfaz}\", " +
            $"\"id_remoto_interfaz\": \"{idRemotoInterfaz}\"}}}}";

        return EnviarSinExcepcionAsync("/api/europielpos/SaveDocumento", json, "HttpPost_SaveDocumento", ct);
    }

    public Task<string> SaveDocumentoAwsAsync(int idTipoDocumento, int idRelacion, int idUsuario, string nombre,
        string referencia, int idSucursal, int noCajaInterfaz, int idRemotoInterfaz,
        string nombreAws, string urlAws, CancellationToken ct = default)
    {
        string json =
            $"{{\"documento\": {{\"id_tipo_documento\": \"{idTipoDocumento}\", \"id_relacion\": \"{idRelacion}\", " +
            $"\"id_usuario_alta\": \"{idUsuario}\", \"nombre\": \"{nombre}\", \"referencia\": \"{referencia}\", " +
            $"\"id_sucursal\": \"{idSucursal}\", \"no_caja_interfaz\": \"{noCajaInterfaz}\", " +
            $"\"id_remoto_interfaz\": \"{idRemotoInterfaz}\", \"nombre_aws\": \"{nombreAws}\", \"url_aws\": \"{urlAws}\"}}}}";

        return EnviarSinExcepcionAsync("/api/europielpos/SaveDocumento", json, "HttpPost_SaveDocumentoAWS", ct);
    }

    // ----- Helpers -----

    /// <summary>
    /// POST con el manejo de respuesta del original: Message == "OK" (o que
    /// empiece con OK) es éxito; cualquier otra cosa devuelve el error como texto.
    /// </summary>
    private async Task<string> EnviarAsync(string servicio, string cuerpo, string? prefijoError,
        bool comparaConStartsWith = false, int? timeoutSegundos = null, CancellationToken ct = default)
    {
        string respuestaCruda = await _api.PostCrudoAsync(servicio, cuerpo,
            new Dictionary<string, string> { ["ClaveBloque"] = _contexto.ClaveBloque }, timeoutSegundos, ct);

        var respuesta = System.Text.Json.JsonSerializer.Deserialize<RespuestaSimple>(respuestaCruda,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        bool exito = comparaConStartsWith
            ? respuesta?.Message?.StartsWith("OK") == true
            : respuesta?.Message == "OK";

        if (!exito)
            return prefijoError is null ? respuestaCruda : $"Error {prefijoError}: {respuestaCruda}";

        return comparaConStartsWith ? respuesta!.Message! : "OK";
    }

    private async Task<string> EnviarSinExcepcionAsync(string servicio, string cuerpo, string prefijoError, CancellationToken ct)
    {
        try
        {
            return await EnviarAsync(servicio, cuerpo, prefijoError, ct: ct);
        }
        catch (Exception ex)
        {
            return $"Error {prefijoError}: {ex.Message}";
        }
    }

    private static string? Fecha(DateTime? fecha) =>
        fecha?.ToString("yyyy-MM-dd HH:mm:ss");
}
