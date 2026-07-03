using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class LogInterfaz
{
    public int Id { get; set; }

    public string? Tipo { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string? Mensaje { get; set; }

    public DateTime? FechaServidor { get; set; }
}
