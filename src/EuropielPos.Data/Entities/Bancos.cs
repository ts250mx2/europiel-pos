using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Bancos
{
    public int IdBanco { get; set; }

    public string? Descripcion { get; set; }

    public int? IdPais { get; set; }
}
