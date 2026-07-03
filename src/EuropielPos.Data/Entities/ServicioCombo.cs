using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ServicioCombo
{
    public int IdDetalle { get; set; }

    public string? Abreviatura { get; set; }

    public string? Descripcion { get; set; }

    public decimal? Monto { get; set; }

    public string? IdsServicio { get; set; }
}
