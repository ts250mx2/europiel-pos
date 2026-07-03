using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class TokenAutorizacion
{
    public int IdDetalle { get; set; }

    public string? Token { get; set; }

    public int? IdPaquete { get; set; }

    public string? Tipo { get; set; }

    public int? IdPaqueteLocal { get; set; }
}
