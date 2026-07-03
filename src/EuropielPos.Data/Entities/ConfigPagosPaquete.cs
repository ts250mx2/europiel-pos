using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ConfigPagosPaquete
{
    public int IdDetalle { get; set; }

    public int? NoServiciosIni { get; set; }

    public int? NoServiciosFin { get; set; }

    public int? MaxPagos { get; set; }

    public int? DiasMaximoPagos { get; set; }

    public int? DiasMaximoDifEntrePagos { get; set; }

    public string? Tipo { get; set; }

    public decimal? VentaMinima { get; set; }
}
