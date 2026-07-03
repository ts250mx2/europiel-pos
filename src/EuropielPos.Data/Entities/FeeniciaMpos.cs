using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class FeeniciaMpos
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public int? TerminalId { get; set; }

    public string? FeecontrollerUrl { get; set; }

    public decimal? Importe { get; set; }

    public string? Caja { get; set; }

    public string? Tienda { get; set; }

    public int? Ticket { get; set; }

    public int? IdLog { get; set; }

    public string? MensajeEnvio { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdBloque { get; set; }

    public string? MensajeRespuesta { get; set; }

    public string? CodigoRespuesta { get; set; }

    public string? Description { get; set; }

    public string? AcquirerBank { get; set; }

    public int? Affiliation { get; set; }

    public string? Aid { get; set; }

    public decimal? Amount { get; set; }

    public string? Arqc { get; set; }

    public string? CardholderName { get; set; }

    public string? IssuerBank { get; set; }

    public string? Last4digits { get; set; }

    public string? Marca { get; set; }

    public string? Merchant { get; set; }

    public string? NumAuth { get; set; }

    public string? OrderId { get; set; }

    public decimal? Propina { get; set; }

    public string? ReceiptId { get; set; }

    public string? Terminal { get; set; }

    public string? TransactionId { get; set; }

    public string? TypeSale { get; set; }

    public string? Id { get; set; }

    public string? SignatureType { get; set; }

    public string? Msi { get; set; }
}
