using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Interfaz
{
    public int? IdInterfaz { get; set; }

    public string? Tipo { get; set; }

    public DateTime? FechaEjecucion { get; set; }

    public DateTime? FechaEjecucionFin { get; set; }

    public int? CantidadRegistros { get; set; }
}
