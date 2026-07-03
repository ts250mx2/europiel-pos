using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class GioPaso
{
    public int Id { get; set; }

    public int? IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdGio { get; set; }

    public string? Nombre { get; set; }

    public string? ClaveInterbancaria { get; set; }

    public string? Banco { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdEstado { get; set; }

    public int? IdCiudad { get; set; }

    public string? Horario { get; set; }

    public bool? EsActivo { get; set; }

    public int? IdRemotoInterfaz { get; set; }

    public int? IdSucursalInterfaz { get; set; }

    public int? NoCajaInterfaz { get; set; }

    public string? NombreCuenta { get; set; }

    public int? IdTurno { get; set; }

    public string? Bloque { get; set; }

    public string? Telefono { get; set; }

    public bool? EsCambioSucursal { get; set; }
}
