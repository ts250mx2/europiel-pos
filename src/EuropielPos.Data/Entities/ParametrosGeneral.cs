using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ParametrosGeneral
{
    public int IdParametroGeneral { get; set; }

    public string? ParametroGeneral { get; set; }

    public string? Valor { get; set; }

    public string? Proveedor { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdBloque { get; set; }

    public int? Activo { get; set; }
}
