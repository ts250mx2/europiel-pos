using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class CitaServicio
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int IdDetalle { get; set; }

    public int? IdCita { get; set; }

    public int? IdCitaLocal { get; set; }

    public int? IdServicio { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdPaqueteLocal { get; set; }

    public string? Tipo { get; set; }
}
