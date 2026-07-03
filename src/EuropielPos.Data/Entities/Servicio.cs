using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Servicio
{
    public int IdServicio { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public string? Descripcion { get; set; }

    public decimal? Precio { get; set; }

    public int? Orden { get; set; }

    public int? IdTipoCita { get; set; }

    public int? IdTipoServicio { get; set; }

    public int? SesionesPaquete { get; set; }

    public decimal? SesionesPaquetePrecio { get; set; }

    public int? Duracion { get; set; }

    public bool? PermitirVenta { get; set; }

    public int? TrDuracionServicio { get; set; }

    public int? TrDuracionLaser { get; set; }

    public int? MaxVentasPorCliente { get; set; }

    public virtual TipoCita? IdTipoCitaNavigation { get; set; }

    public virtual TipoServicio? IdTipoServicioNavigation { get; set; }

    public virtual ICollection<ServicioSucursal> ServicioSucursal { get; set; } = new List<ServicioSucursal>();
}
