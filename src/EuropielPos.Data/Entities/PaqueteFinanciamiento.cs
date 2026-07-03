using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class PaqueteFinanciamiento
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdFinanciemiento { get; set; }

    public int? IdPaqueteLocal { get; set; }

    public int? IdPaquete { get; set; }

    public int? NumPago { get; set; }

    public decimal? Monto { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Tipo { get; set; }

    public int? BanderaManual { get; set; }
}
