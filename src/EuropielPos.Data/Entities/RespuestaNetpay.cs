using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class RespuestaNetpay
{
    public int Id { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public DateTime? Fecha { get; set; }

    public string? OrderId { get; set; }

    public string? TransactionId { get; set; }

    public string? CardNumber { get; set; }

    public string? ResponseCode { get; set; }

    public string? ResponseMsg { get; set; }

    public string? ResponseText { get; set; }

    public string? Arqc { get; set; }

    public string? Cvm { get; set; }

    public string? Aid { get; set; }

    public string? MerchantId { get; set; }

    public string? AuthCode { get; set; }

    public string? CustomerName { get; set; }

    public string? BankName { get; set; }

    public string? CardTypeName { get; set; }

    public string? CardType { get; set; }

    public string? PosentryMode { get; set; }

    public decimal? Monto { get; set; }

    public string? CardToken { get; set; }

    public string? Procesador { get; set; }

    public int? IdPaciente { get; set; }

    public string? Paciente { get; set; }

    public string? Promocion { get; set; }

    public string? Tipo { get; set; }
}
