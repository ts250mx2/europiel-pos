namespace EuropielPos.Domain.Pagos;

// DTOs del procesador Feenicia/Serti (port de modFeenicia.vb).
// camelCase/snake_case conservados: son contratos de APIs externas.

#pragma warning disable IDE1006
/// <summary>Suscripción tal como viaja al API central (snake_case).</summary>
public class FeeniciaSuscripcionSimple
{
    public int id_local { get; set; }

    public DateTime? fecha_modificacion_local { get; set; }

    public DateTime? fecha_interfaz { get; set; }

    public int id_subscripcion { get; set; }

    public string? mensaje_respuesta { get; set; }

    public string? codigo_respuesta { get; set; }

    public string? id_plan { get; set; }

    public int? plan_unit { get; set; }

    public string? payload { get; set; }

    public int id_local_paquete { get; set; }

    public int? id_paquete { get; set; }

    public int? id_sucursal { get; set; }

    public int? id_bloque { get; set; }

    public bool activo { get; set; }
}

/// <summary>Alta de suscripción en el API de Serti (camelCase).</summary>
public class SubscribeFeenicia
{
    public string? pan { get; set; }

    public string? cvv { get; set; }

    public string? month { get; set; }

    public string? year { get; set; }

    public string? cardHolderName { get; set; }

    public string? email { get; set; }

    public string? amount { get; set; }

    public string? startDate { get; set; }

    public string? reference { get; set; }

    public PlanSubscribe? plan { get; set; }

    public class PlanSubscribe
    {
        public string? idPlan { get; set; }

        public int planUnit { get; set; }
    }
}

public class RespuestaSuscribeFeenicia
{
    public string? responseCode { get; set; }

    public string? responseMessage { get; set; }

    public int idSubscription { get; set; }
}

/// <summary>Petición/respuesta de la terminal MPOS de Feenicia.</summary>
public class MposRequest
{
    public decimal importe { get; set; }

    public string? id { get; set; }

    public string? caja { get; set; }

    public string? customerInfo { get; set; }
}

public class MposResponse
{
    public string? responseCode { get; set; }

    public string? description { get; set; }

    public string? acquirerBank { get; set; }

    public int affiliation { get; set; }

    public string? aid { get; set; }

    public decimal amount { get; set; }

    public string? arqc { get; set; }

    public string? cardholderName { get; set; }

    public string? issuerBank { get; set; }

    public string? last4digits { get; set; }

    public string? marca { get; set; }

    public string? merchant { get; set; }

    public string? numAuth { get; set; }

    public string? orderId { get; set; }

    public decimal propina { get; set; }

    public string? receiptId { get; set; }

    public string? terminal { get; set; }

    public string? transactionId { get; set; }

    public string? typeSale { get; set; }

    public string? id { get; set; }

    public string? signatureType { get; set; }

    public string? msi { get; set; }
}
#pragma warning restore IDE1006
