using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class TipoFalla
{
    public int IdTipoFalla { get; set; }

    public string? Descripcion { get; set; }

    public string? TipoRequerimiento { get; set; }

    public int? TiempoRespuesta { get; set; }
}
