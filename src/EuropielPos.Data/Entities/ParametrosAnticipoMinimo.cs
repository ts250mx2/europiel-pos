using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ParametrosAnticipoMinimo
{
    public int IdParametro { get; set; }

    public int IdSucursal { get; set; }

    public string Bloque { get; set; } = null!;

    public int IdUsuarioAlta { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<ParametrosAnticipoMinimoDetalle> ParametrosAnticipoMinimoDetalle { get; set; } = new List<ParametrosAnticipoMinimoDetalle>();
}
