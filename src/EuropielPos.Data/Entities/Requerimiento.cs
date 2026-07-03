using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Requerimiento
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdRequerimiento { get; set; }

    public int? Folio { get; set; }

    public int? IdSucursal { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public string? Concepto { get; set; }

    public decimal? Monto { get; set; }

    public string? Dirigido { get; set; }

    public DateTime? FechaAlta { get; set; }

    public int? IdUsuarioAlta { get; set; }

    public string? BloqueAlta { get; set; }

    public DateTime? FechaTermino { get; set; }

    public int? IdUsuarioTermino { get; set; }

    public string? BloqueTermino { get; set; }

    public bool? EsBorrado { get; set; }

    public int? IdUsuarioBorro { get; set; }

    public DateTime? FechaBorro { get; set; }

    public string? BloqueBorro { get; set; }

    public string? Tipo { get; set; }

    public int? IdTipoFalla { get; set; }
}
