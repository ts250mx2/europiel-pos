using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class TipoCita
{
    public int IdTipoCita { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Servicio> Servicio { get; set; } = new List<Servicio>();
}
