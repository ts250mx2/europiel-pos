using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class MobileHorariosCierreJuntas
{
    public int Id { get; set; }

    public int? IdSucursal { get; set; }

    public DateTime? Fecha { get; set; }

    public DateTime? FechaFin { get; set; }
}
