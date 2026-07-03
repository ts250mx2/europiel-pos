using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class LogSucursalIntentosCheckin
{
    public int Id { get; set; }

    public int? BranchId { get; set; }

    public int? IdSucursal { get; set; }

    public string? Tipo { get; set; }

    public DateTime? GioInicioTurno { get; set; }

    public int? CantidadPosponer { get; set; }

    public int? IdUsuarioRespondioNinguno { get; set; }

    public int? IdUsuarioRegistro { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Estatus { get; set; }
}
