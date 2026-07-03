using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class MobileHorariosCierreJuntasPaso
{
    public int Id { get; set; }

    public int? IdSucursal { get; set; }

    public DateTime? Fecha { get; set; }

    public DateTime? FechaFin { get; set; }
}
