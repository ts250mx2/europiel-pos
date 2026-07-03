using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ControlScripts
{
    public int IdDetalle { get; set; }

    public DateTime? Fecha { get; set; }

    public string? NombreArchivo { get; set; }
}
