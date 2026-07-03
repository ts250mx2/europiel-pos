using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class PagoCajaDetalle
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdPagoDetalle { get; set; }

    public int? IdPagoLocal { get; set; }

    public int? IdPago { get; set; }

    public int? IdProducto { get; set; }

    public decimal? Monto { get; set; }

    public int? IdLocalPaciente { get; set; }

    public int? IdPaciente { get; set; }

    public int? IdPaqueteLocal { get; set; }

    public int? IdPaquete { get; set; }

    public int? PagoRecuperacion { get; set; }

    public int? IdRecuperador { get; set; }

    public virtual PagoCaja? IdPagoLocalNavigation { get; set; }
}
