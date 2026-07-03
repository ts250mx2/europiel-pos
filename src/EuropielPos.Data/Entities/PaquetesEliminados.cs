using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class PaquetesEliminados
{
    public int Id { get; set; }

    public int? IdPaquete { get; set; }

    public DateTime? FechaRegistro { get; set; }
}
