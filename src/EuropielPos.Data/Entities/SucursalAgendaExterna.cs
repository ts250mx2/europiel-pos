using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class SucursalAgendaExterna
{
    public int IdDetalle { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdSucursalAfin { get; set; }

    public virtual Sucursal? IdSucursalAfinNavigation { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }
}
