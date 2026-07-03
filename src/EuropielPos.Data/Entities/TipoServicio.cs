using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class TipoServicio
{
    public int IdTipoServicio { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Servicio> Servicio { get; set; } = new List<Servicio>();
}
