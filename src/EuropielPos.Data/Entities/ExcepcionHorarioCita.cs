using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ExcepcionHorarioCita
{
    public int IdDetalle { get; set; }

    public int? IdSucursal { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Tipo { get; set; }

    public string? Mensaje { get; set; }
}
