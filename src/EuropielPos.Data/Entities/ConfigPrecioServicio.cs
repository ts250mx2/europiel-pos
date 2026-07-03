using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ConfigPrecioServicio
{
    public int IdDetalle { get; set; }

    public int? Orden { get; set; }

    public string? TipoDescuento { get; set; }

    public decimal? Descuento { get; set; }

    public string? Tipo { get; set; }

    public int? NoServiciosIni { get; set; }

    public int? NoServiciosFin { get; set; }
}
