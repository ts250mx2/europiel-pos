using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class GioTurno
{
    public int IdTurno { get; set; }

    public DateTime? HoraInicio { get; set; }

    public DateTime? HoraFin { get; set; }

    public string? NombreTurno { get; set; }
}
