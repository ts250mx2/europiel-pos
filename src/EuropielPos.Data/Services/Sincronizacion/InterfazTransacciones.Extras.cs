using System.Text.Json;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Métodos finales de <c>modInterfaz.vb</c>: anticipos mínimos, bancos POS,
/// WhatsApp y cobro de validación de tarjeta.
/// </summary>
public interface IInterfazExtras
{
    Task<string> GetConfigAnticiposMinimosAsync(int claveBloque, int sucursal, CancellationToken ct = default);

    Task<string> GetBancosPosAsync(int claveBloque, int idPais, CancellationToken ct = default);

    Task<string> SendMensajeAsync(int idPaquete, bool clienteLiquidaEnSucursal, DateTime fechaVisitaCliente, int idPais, CancellationToken ct = default);

    Task<RespuestaCobroValidacionTarjeta> CobroValidacionTarjetaAsync(RequestValidacionTarjeta peticion, CancellationToken ct = default);
}

public partial class InterfazTransaccionesService : IInterfazExtras
{
    /// <remarks>La fecha devuelta es la hora local (el SP del servidor no
    /// manda fecha_servidor en este endpoint), igual que el original.</remarks>
    public async Task<string> GetConfigAnticiposMinimosAsync(int claveBloque, int sucursal, CancellationToken ct = default)
    {
        try
        {
            string crudo = await _api.PostCrudoAsync("/api/europielpos/GetConfigAnticiposMinimos",
                $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\" }}",
                new Dictionary<string, string>
                {
                    ["ClaveBloque"] = claveBloque.ToString(),
                    ["Sucursal"] = sucursal.ToString(),
                }, ct: ct);

            using var doc = JsonDocument.Parse(crudo);
            var valor = doc.RootElement.TryGetProperty("Value", out var v) ? v : default;

            await _db.Database.ExecuteSqlAsync($"EXEC limpiar_parametros_anticipo_paso", ct);

            if (valor.ValueKind == JsonValueKind.Object
                && valor.TryGetProperty("ParametrosAnticipoMinimoDetalle", out var lista)
                && lista.GetArrayLength() > 0)
            {
                await BulkJson.BulkInsertAsync(_db, "parametros_anticipo_paso", lista, ct: ct);
                await _db.Database.ExecuteSqlAsync($"EXEC procesa_anticipo_minimo_paso", ct);
            }

            return "OK|" + Fecha(DateTime.Now);
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetConfigAnticiposMinimos: " + ex.Message, ex);
        }
    }

    public async Task<string> GetBancosPosAsync(int claveBloque, int idPais, CancellationToken ct = default)
    {
        try
        {
            string crudo = await _api.PostCrudoAsync("/api/europielpos/GetBancosPOS",
                $"{{ \"IdPais\": \"{idPais}\" }}",
                new Dictionary<string, string> { ["ClaveBloque"] = claveBloque.ToString() }, ct: ct);

            using var doc = JsonDocument.Parse(crudo);
            var valor = doc.RootElement.TryGetProperty("Value", out var v) ? v : default;

            await _db.Database.ExecuteSqlAsync($"EXEC limpiar_bancos_pos_paso", ct);

            if (valor.ValueKind == JsonValueKind.Object
                && valor.TryGetProperty("BancosPOS", out var lista)
                && lista.GetArrayLength() > 0)
            {
                await BulkJson.BulkInsertAsync(_db, "bancos_pos_paso", lista, ct: ct);
                await _db.Database.ExecuteSqlAsync($"EXEC procesa_bancos_pos_paso", ct);
            }

            return "OK|" + Fecha(DateTime.Now);
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetBancosPOS: " + ex.Message, ex);
        }
    }

    /// <remarks>El JSON del mensaje lo arma el SP <c>prepara_json_para_enviar_whatsapp</c>.</remarks>
    public async Task<string> SendMensajeAsync(int idPaquete, bool clienteLiquidaEnSucursal, DateTime fechaVisitaCliente, int idPais, CancellationToken ct = default)
    {
        try
        {
            var resultado = await ProcedimientoAlmacenado.EscalarAsync(_db, "prepara_json_para_enviar_whatsapp",
                new Dictionary<string, object?>
                {
                    ["@id_paquete"] = idPaquete,
                    ["@telefono_prueba"] = string.Empty,
                    ["@cliente_liquida_en_sucursal"] = clienteLiquidaEnSucursal,
                    ["@fecha_visita_cliente"] = fechaVisitaCliente.ToString("yyyyMMdd HH:mm"),
                    ["@id_pais"] = idPais,
                }, ct);

            string cuerpo = Convert.ToString(resultado) ?? string.Empty;

            string crudo = await _api.PostCrudoAsync("/api/europielpos/SendWhatsapp", cuerpo,
                new Dictionary<string, string> { ["ClaveBloque"] = _contexto.ClaveBloque }, ct: ct);

            var respuesta = JsonSerializer.Deserialize<RespuestaSimple>(crudo,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return respuesta?.Message == "OK" ? "OK" : "Error HttpPost_SendMensaje: " + crudo;
        }
        catch (Exception ex)
        {
            return "Error HttpPost_SendMensaje: " + ex.Message;
        }
    }

    public async Task<RespuestaCobroValidacionTarjeta> CobroValidacionTarjetaAsync(RequestValidacionTarjeta peticion, CancellationToken ct = default)
    {
        try
        {
            string cuerpo = JsonSerializer.Serialize(peticion);

            string crudo = await _api.PostCrudoAsync("/api/europielpos/CobroValidacionTarjeta", cuerpo,
                new Dictionary<string, string> { ["ClaveBloque"] = _contexto.ClaveBloque }, ct: ct);

            return JsonSerializer.Deserialize<RespuestaCobroValidacionTarjeta>(crudo,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new RespuestaCobroValidacionTarjeta();
        }
        catch (Exception ex)
        {
            return new RespuestaCobroValidacionTarjeta
            {
                Estatus = "Error " + ex.Message,
                Message = ex.Message,
            };
        }
    }

    /// <summary>
    /// Monto aleatorio (1.0 a 10.0 en pasos de 0.5) para el cargo de
    /// validación de tarjeta. Se replica el detalle de que el original nunca
    /// elegía el último valor (10) porque Random.Next excluye el tope.
    /// </summary>
    public static decimal ObtenerMontoRandom()
    {
        decimal[] valores = [1m, 1.5m, 2m, 2.5m, 3m, 3.5m, 4m, 4.5m, 5m, 5.5m, 6m, 6.5m, 7m, 7.5m, 8m, 8.5m, 9m, 9.5m, 10m];
        return valores[Random.Shared.Next(0, valores.Length - 1)];
    }
}
