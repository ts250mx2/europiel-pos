using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Estados
{
    public int IdEstado { get; set; }

    public int? Codigo { get; set; }

    public int? IdPais { get; set; }

    public string? Estado { get; set; }

    public string? Sigla { get; set; }
}
