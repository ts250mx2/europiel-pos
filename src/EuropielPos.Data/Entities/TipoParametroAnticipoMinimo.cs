using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class TipoParametroAnticipoMinimo
{
    public int IdTipo { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<ParametrosAnticipoMinimoDetalle> ParametrosAnticipoMinimoDetalle { get; set; } = new List<ParametrosAnticipoMinimoDetalle>();
}
