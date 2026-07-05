namespace EuropielPos.Domain.Sincronizacion;

/// <summary>Respuesta del servicio de reconocimiento facial (Reko).</summary>
public class RespuestaFromReko
{
    public string? Message { get; set; }

    public int? IdGio { get; set; }

    public string? Picture { get; set; }

    public string? Name { get; set; }

    public string? IdFeedback { get; set; }

    public string? Estatus { get; set; }

    public string? MsgEstatus { get; set; }
}

public class RespuestaCheckinReko
{
    public string? MessageFromRequest { get; set; }

    public string? Message { get; set; }

    public int Result { get; set; }
}
