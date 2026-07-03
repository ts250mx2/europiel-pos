using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class BonosPorSucursal
{
    public int IdBono { get; set; }

    public int? IdSucursal { get; set; }

    public string? TipoBono { get; set; }

    public decimal? Venta { get; set; }

    public decimal? Premio { get; set; }

    public string? Tipo { get; set; }
}
