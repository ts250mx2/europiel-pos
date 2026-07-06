namespace EuropielPos.Domain.Sincronizacion;

/// <summary>
/// Identidad de esta caja frente al servidor central. Sustituye a los
/// globales de <c>modGeneral.vb</c> (Bloque, IdSucursal, NoCaja...). Se llena
/// al iniciar sesión desde la tabla <c>parametro</c> y se registra como
/// singleton en el contenedor.
/// </summary>
public class ContextoPos
{
    public string Bloque { get; set; } = string.Empty;

    public string ClaveBloque { get; set; } = "1";

    public int IdBloque { get; set; }

    public int IdSucursal { get; set; }

    public int? NoCaja { get; set; }

    public int VersionSistema { get; set; }

    public int EsEuroskin { get; set; }

    public int IdPais { get; set; }

    /// <summary>Ventana de desbloqueo de agenda otorgada por un supervisor
    /// (modGeneral.FechaDesbloqueo del original).</summary>
    public DateTime? FechaDesbloqueo { get; set; }
}

/// <summary>Respuesta simple del API: solo <c>Message</c>.</summary>
public class RespuestaSimple
{
    public string? Message { get; set; }
}
