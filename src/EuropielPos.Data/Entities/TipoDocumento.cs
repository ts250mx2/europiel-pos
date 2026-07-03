using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class TipoDocumento
{
    public int IdTipoDocumento { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public string? Abreviatura { get; set; }

    public string? Descripcion { get; set; }

    public bool? EsBorrado { get; set; }

    public virtual ICollection<Documento> Documento { get; set; } = new List<Documento>();
}
