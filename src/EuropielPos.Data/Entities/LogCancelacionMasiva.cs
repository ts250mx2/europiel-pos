using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class LogCancelacionMasiva
{
    public int IdLog { get; set; }

    public DateTime? Fecha { get; set; }

    public int? IdUsuario { get; set; }

    public string? Descripcion { get; set; }

    public string? IdCitas { get; set; }

    public DateTime? FechaVisualizacion { get; set; }

    public int? IdSucursal { get; set; }

    public DateTime? HoraInicio { get; set; }

    public DateTime? HoraFin { get; set; }

    public string? DiaCancelacion { get; set; }
}
