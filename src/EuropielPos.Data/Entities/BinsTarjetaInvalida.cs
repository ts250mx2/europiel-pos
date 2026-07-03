using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class BinsTarjetaInvalida
{
    public string? Tarjeta { get; set; }

    public string? Banco { get; set; }

    public int? IdPais { get; set; }
}
