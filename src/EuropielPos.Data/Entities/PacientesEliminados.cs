using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class PacientesEliminados
{
    public int Id { get; set; }

    public int? IdPaciente { get; set; }

    public DateTime? FechaRegistro { get; set; }
}
