using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class GioAsistenciaPaso
{
    public int Id { get; set; }

    public int? IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdDetalle { get; set; }

    public int? IdGioLocal { get; set; }

    public int? IdGio { get; set; }

    public int? IdSucursal { get; set; }

    public DateTime? Fecha { get; set; }

    public int? IdTurno { get; set; }

    public DateTime? HoraInicio { get; set; }

    public DateTime? HoraFin { get; set; }

    public int? IdUsuarioRegistro { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? IdRemotoInterfaz { get; set; }

    public int? IdSucursalInterfaz { get; set; }

    public int? NoCajaInterfaz { get; set; }

    public string? Bloque { get; set; }

    public string? Horario { get; set; }
}
