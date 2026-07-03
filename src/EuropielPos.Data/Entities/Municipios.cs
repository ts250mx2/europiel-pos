using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Municipios
{
    public int IdMunicipio { get; set; }

    public int? IdEstado { get; set; }

    public int? IdPais { get; set; }

    public int? Codigo { get; set; }

    public string? Municipio { get; set; }
}
