using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class LogErrores
{
    public int IdLog { get; set; }

    public string? Tipo { get; set; }

    public string? Mensaje { get; set; }

    public DateTime? Fecha { get; set; }
}
