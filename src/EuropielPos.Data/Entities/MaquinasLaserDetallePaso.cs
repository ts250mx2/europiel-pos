using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class MaquinasLaserDetallePaso
{
    public int Id { get; set; }

    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int IdMaquinaDet { get; set; }

    public int IdMaquina { get; set; }

    public string Serie { get; set; } = null!;

    public string? NumSerieDispositivo { get; set; }

    public int? IdSucursal { get; set; }

    public string? Bloque { get; set; }

    public string? Observaciones { get; set; }

    public int? NoCabina { get; set; }

    public string? IpAddress { get; set; }

    public string? MacAddress { get; set; }

    public int? IdRemotoInterfaz { get; set; }

    public int? IdSucursalInterfaz { get; set; }

    public int? NoCajaInterfaz { get; set; }
}
