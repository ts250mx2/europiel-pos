using System.Text.Json;

namespace EuropielPos.Domain.Sincronizacion;

/// <summary>
/// Petición de cobro de validación de tarjeta (port de
/// <c>request_validacion_tarjeta</c>). Mezcla PascalCase con dos campos en
/// minúsculas porque así lo espera el API.
/// </summary>
#pragma warning disable IDE1006
public class RequestValidacionTarjeta
{
    public string? CardNumber { get; set; }

    public string? Expiration { get; set; }

    public string? Amount { get; set; }

    public string? Currency { get; set; }

    public string? Cvv2 { get; set; }

    public string? BranchId { get; set; }

    public string? Promotion { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Zipcode { get; set; }

    public string? IdSucursal { get; set; }

    public string? NoCaja { get; set; }

    public string? Bloque { get; set; }

    public string? IdBloque { get; set; }

    public int IdUsuario { get; set; }

    public int id_paciente { get; set; }

    public string? paciente { get; set; }

    public string? TipoCobro { get; set; }
}
#pragma warning restore IDE1006

public class RespuestaCobroValidacionTarjeta
{
    public string? Message { get; set; }

    public string? Estatus { get; set; }

    public JsonElement? Value { get; set; }
}
