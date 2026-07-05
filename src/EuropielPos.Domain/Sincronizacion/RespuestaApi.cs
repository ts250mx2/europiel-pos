namespace EuropielPos.Domain.Sincronizacion;

/// <summary>
/// Envoltura estándar de las respuestas del API central
/// (<c>/api/europielpos/*</c>). En el VB original había ~30 clases
/// <c>responseX</c> idénticas; aquí una sola genérica.
/// </summary>
public class RespuestaApi<TValue>
{
    public string? Message { get; set; }

    public TValue? Value { get; set; }
}

// ----- Catálogos de bloques y sucursales (payloads del API, PascalCase) -----

public class ValorSucursalBloque
{
    public List<SucursalBloqueApi>? SucursalBloque { get; set; }
}

public class ValorBloque
{
    public List<BloqueApi>? Bloques { get; set; }
}

public class SucursalBloqueApi
{
    public int IdSucursal { get; set; }

    public string? Sucursal { get; set; }

    public int IdBloque { get; set; }

    public int ClaveBloque { get; set; }

    public string? NombreBloque { get; set; }
}

public class BloqueApi
{
    public int IdBloque { get; set; }

    public int ClaveBloque { get; set; }

    public string? Nombre { get; set; }

    /// <summary>Formato <c>clave|id</c> que usaban los combos del original.</summary>
    public string DetalleBloque => $"{ClaveBloque}|{IdBloque}";
}
