using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class CitaBorrada
{
    public int Id { get; set; }

    public int? IdCita { get; set; }

    public int? IdRemotoInterfaz { get; set; }

    public int? NoCajaInterfaz { get; set; }

    public int? IdSucursalInterfaz { get; set; }

    public int? Procesada { get; set; }

    public DateTime? Fecha { get; set; }
}
