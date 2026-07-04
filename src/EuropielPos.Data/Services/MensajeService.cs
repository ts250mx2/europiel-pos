namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>MensajeBL.vb</c> — arma el JSON del mensaje de WhatsApp de
/// bienvenida/recordatorio vía el SP <c>prepara_json_para_enviar_whatsapp</c>.
/// </summary>
public interface IMensajeService
{
    Task<string> PreparaJsonParaEnviarWhatsappAsync(int idPaquete, bool clienteLiquidaEnSucursal, DateTime fechaVisitaCliente, int idPais, CancellationToken ct = default);
}

public class MensajeService : IMensajeService
{
    private readonly PosDbContext _db;

    public MensajeService(PosDbContext db) => _db = db;

    public async Task<string> PreparaJsonParaEnviarWhatsappAsync(int idPaquete, bool clienteLiquidaEnSucursal, DateTime fechaVisitaCliente, int idPais, CancellationToken ct = default)
    {
        var resultado = await ProcedimientoAlmacenado.EscalarAsync(_db, "prepara_json_para_enviar_whatsapp",
            new Dictionary<string, object?>
            {
                ["@id_paquete"] = idPaquete,
                ["@telefono_prueba"] = string.Empty,
                ["@cliente_liquida_en_sucursal"] = clienteLiquidaEnSucursal,
                // El original usaba ConvierteFechaHoraSQL: formato yyyyMMdd HH:mm.
                ["@fecha_visita_cliente"] = fechaVisitaCliente.ToString("yyyyMMdd HH:mm"),
                ["@id_pais"] = idPais,
            }, ct);

        return Convert.ToString(resultado) ?? string.Empty;
    }
}
