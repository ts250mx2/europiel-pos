using System.Text.Json;
using System.Text.Json.Serialization;

namespace EuropielPos.Domain.Reportes;

// DTOs de GetReporteDiario / GetReporteConsultaCaja (Objetos.vb y modInterfaz.vb
// del original). Los nombres en minúsculas replican el JSON del API central;
// las listas contenedoras van en PascalCase como las manda el servidor.
#pragma warning disable IDE1006

/// <summary>
/// Fechas del API central: acepta ISO-8601 y el formato legado
/// <c>/Date(ms)/</c> de JavaScriptSerializer (mismo criterio que BulkJson).
/// </summary>
public class FechaApiConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Convierte(reader.GetString());

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));

    internal static DateTime Convierte(string? s)
    {
        if (string.IsNullOrEmpty(s))
            return default;

        if (s.StartsWith("/Date(", StringComparison.Ordinal))
        {
            long ms = long.Parse(s[6..s.IndexOfAny(['+', '-', ')'], 6)]);
            return DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime;
        }

        return DateTime.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
    }
}

/// <summary>Variante nullable de <see cref="FechaApiConverter"/>.</summary>
public class FechaApiNullableConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        string? s = reader.GetString();
        return string.IsNullOrEmpty(s) ? null : FechaApiConverter.Convierte(s);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}

// ---------------------------------------------------------------------------
// Reporte diario
// ---------------------------------------------------------------------------

/// <summary>Respuesta de GetReporteDiario (responseReporteDiario del original).</summary>
public class RespuestaReporteDiario
{
    public string? Message { get; set; }

    public ValorReporteDiario? Value { get; set; }
}

public class ValorReporteDiario
{
    public ReporteDiarioApi? ReporteDiario { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha_servidor { get; set; }
}

/// <summary>reporte_diario del original: bloques que el cliente vuelca a HTML.</summary>
public class ReporteDiarioApi
{
    public List<RdCobranzaNormal> CobranzaNormal { get; set; } = [];

    public List<RdTotalCobranzaNormal> TotalCobranzaNormal { get; set; } = [];

    public List<RdPago> PagosHoy { get; set; } = [];

    public List<RdPago> PagosManiana { get; set; } = [];

    public List<RdPago> PagosAyer { get; set; } = [];

    public List<RdCobranzaAtrasadaNoPaso> CobranzaAtrasadaNoPaso { get; set; } = [];

    public List<RdSucursal> Sucursales { get; set; } = [];

    public List<RdCitaHoy> CitasHoy { get; set; } = [];

    public List<RdComisionNoSePagara> ComisionesNoSePagaran { get; set; } = [];

    public List<RdClienteParaValoracion> ClientesParaValoracion { get; set; } = [];
}

public class RdCobranzaNormal
{
    public string? nombre_paciente_1 { get; set; }

    public int num_pago { get; set; }

    public decimal monto_pendiente { get; set; }

    public string? tipo_cobranza { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha_pago { get; set; }

    public string? forma_pago { get; set; }

    public string? tarjeta_numero { get; set; }

    public string? tarjeta_tipo { get; set; }

    public string? tarjeta_cvv { get; set; }

    public string? tarjeta_fecha_venc { get; set; }

    public string? estatus { get; set; }

    public string? sucursal { get; set; }

    public bool tiene_citas_futuro { get; set; }

    [JsonConverter(typeof(FechaApiNullableConverter))]
    public DateTime? no_paso_fecha { get; set; }

    public string? comentarios { get; set; }

    public int orden { get; set; }

    public bool paquete_pago_automatico { get; set; }

    public string? telefonos { get; set; }

    public int es_negrita { get; set; }
}

public class RdTotalCobranzaNormal
{
    public decimal total_monto_pendiente { get; set; }

    public decimal total_negritas { get; set; }
}

/// <summary>rd_pagos_hoy / rd_pagos_maniana / rd_pagos_ayer: el original tenía
/// tres clases idénticas campo por campo; aquí basta una.</summary>
public class RdPago
{
    public int id_paquete { get; set; }

    public string? nombre_paciente { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha_inicio { get; set; }

    public decimal monto_pendiente { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha_pago { get; set; }

    public string? comentarios { get; set; }

    public string? sucursal { get; set; }

    public string? telefonos { get; set; }

    public decimal costo_total { get; set; }

    public decimal monto_pagado { get; set; }

    public int reagenda { get; set; }

    public string? clave_acceso { get; set; }
}

public class RdCobranzaAtrasadaNoPaso
{
    public string? referencia_prosa { get; set; }

    public string? nombre_paciente_1 { get; set; }

    public int num_pago { get; set; }

    public decimal monto_pendiente { get; set; }

    public string? tipo_cobranza { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha_pago { get; set; }

    public string? forma_pago { get; set; }

    public string? tarjeta_numero { get; set; }

    public string? tarjeta_tipo { get; set; }

    public string? tarjeta_cvv { get; set; }

    public string? tarjeta_fecha_venc { get; set; }

    public string? estatus { get; set; }

    public string? sucursal { get; set; }

    public bool tiene_citas_futuro { get; set; }

    [JsonConverter(typeof(FechaApiNullableConverter))]
    public DateTime? no_paso_fecha { get; set; }

    public string? comentarios { get; set; }

    public int orden { get; set; }

    public bool paquete_pago_automatico { get; set; }

    [JsonConverter(typeof(FechaApiNullableConverter))]
    public DateTime? proxima_cita { get; set; }
}

public class RdSucursal
{
    public int id_sucursal { get; set; }

    public string? descripcion { get; set; }
}

public class RdCitaHoy
{
    public string? nombre_paciente { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha_inicio { get; set; }

    public string? telefono { get; set; }

    public string? clave_acceso { get; set; }

    public string? sucursal { get; set; }
}

public class RdComisionNoSePagara
{
    public string? nombre_paciente { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha_compra { get; set; }

    public string? vendedor { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha { get; set; }

    public string? mensaje { get; set; }

    public string? sucursal { get; set; }
}

public class RdClienteParaValoracion
{
    public string? nombre_paciente { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha_inicio { get; set; }

    public string? telefono { get; set; }

    public string? clave_acceso { get; set; }

    public string? sucursal { get; set; }

    public int citas_asistidas { get; set; }
}

// ---------------------------------------------------------------------------
// Consulta de caja
// ---------------------------------------------------------------------------

/// <summary>Respuesta de GetReporteConsultaCaja (responseReporteConsultaCaja).</summary>
public class RespuestaReporteConsultaCaja
{
    public string? Message { get; set; }

    public ValorReporteConsultaCaja? Value { get; set; }
}

public class ValorReporteConsultaCaja
{
    public ReporteConsultaCajaApi? ReporteConsultaCaja { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha_servidor { get; set; }
}

/// <summary>reporte_consulta_caja del original.</summary>
public class ReporteConsultaCajaApi
{
    public List<RcConsulta> Consulta { get; set; } = [];

    public List<RcTotales> Totales { get; set; } = [];

    public List<RcFormaPago> FormaPago { get; set; } = [];

    public List<RcTotalDepositar> TotalDepositar { get; set; } = [];

    public List<RcTerminal> Terminal { get; set; } = [];

    public List<RcTerminalResumen> TerminalResumen { get; set; } = [];
}

public class RcConsulta
{
    public int id { get; set; }

    public int id_pago { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha { get; set; }

    public string? tipo_recibo { get; set; }

    public string? nombre { get; set; }

    public decimal subtotal { get; set; }

    public decimal iva { get; set; }

    public decimal total { get; set; }

    public decimal saldo { get; set; }

    public string? usuario { get; set; }

    public string? comentario { get; set; }

    public string? sucursal { get; set; }

    public string? terminal { get; set; }

    public string? cobrado_por { get; set; }

    public string? info_pago { get; set; }
}

public class RcTotales
{
    public decimal total { get; set; }

    public decimal saldo { get; set; }
}

public class RcFormaPago
{
    public decimal total_pagado { get; set; }

    public decimal pago_efectivo { get; set; }

    public decimal pago_tc { get; set; }

    public decimal pago_td { get; set; }

    public decimal sin_identificar { get; set; }
}

public class RcTotalDepositar
{
    public decimal pago_tctd { get; set; }

    public decimal pago_netpay { get; set; }

    public decimal pago_paypoint { get; set; }

    public decimal pago_prosa { get; set; }

    public decimal total { get; set; }
}

public class RcTerminal
{
    public string? nombre { get; set; }

    [JsonConverter(typeof(FechaApiConverter))]
    public DateTime fecha { get; set; }

    public string? terminal { get; set; }

    public string? usuario { get; set; }

    public decimal total { get; set; }

    public decimal total_honduras { get; set; }
}

public class RcTerminalResumen
{
    public string? terminal { get; set; }

    public int pagos { get; set; }

    public decimal total { get; set; }
}

#pragma warning restore IDE1006
