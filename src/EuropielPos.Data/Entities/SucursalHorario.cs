using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class SucursalHorario
{
    public int IdDetalle { get; set; }

    public int? IdSucursal { get; set; }

    public int? Dia { get; set; }

    public DateTime? Apertura { get; set; }

    public DateTime? Cierre { get; set; }

    public bool? BloqueoXComida { get; set; }

    public bool? BloqueoXDescanso { get; set; }

    public DateTime? AperturaRecepcion { get; set; }

    public DateTime? CierreRecepcion { get; set; }

    public DateTime? FechaInicioEspecial { get; set; }

    public DateTime? FechaFinEspecial { get; set; }

    public bool? BloqueoXComidaMediaHora { get; set; }
}
