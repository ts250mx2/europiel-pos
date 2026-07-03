using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ServicioSucursal
{
    public int IdDetalle { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdServicio { get; set; }

    public int? IdSucursal { get; set; }

    public decimal? Precio { get; set; }

    public virtual Servicio? IdServicioNavigation { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }
}
