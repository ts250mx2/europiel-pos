using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class TipoIdentificacion
{
    public int IdTipoIdentificacion { get; set; }

    public string? Clave { get; set; }

    public string? Descripcion { get; set; }

    public int? IdPais { get; set; }
}
