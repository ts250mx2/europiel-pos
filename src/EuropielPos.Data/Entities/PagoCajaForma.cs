using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class PagoCajaForma
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdPagoLocal { get; set; }

    public int? IdPago { get; set; }

    public decimal? Pago { get; set; }

    public decimal? PagoEfectivo { get; set; }

    public decimal? PagoTc { get; set; }

    public decimal? PagoTd { get; set; }

    public decimal? PagoSinCategoria { get; set; }

    public decimal? PagoCa { get; set; }

    public decimal? PagoTransferencia { get; set; }

    public virtual PagoCaja? IdPagoLocalNavigation { get; set; }
}
