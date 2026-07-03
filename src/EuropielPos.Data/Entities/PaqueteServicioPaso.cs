using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class PaqueteServicioPaso
{
    public int Id { get; set; }

    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdPaqueteServicio { get; set; }

    public int? IdLocalPaquete { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdServicio { get; set; }

    public int? Cantidad { get; set; }

    public int? IdPacienteLocal { get; set; }

    public int? IdPaciente { get; set; }

    public int? EsGratis { get; set; }

    public string? Estatus { get; set; }

    public int? IdRemotoInterfazPaquete { get; set; }

    public int? IdSucursalInterfazPaquete { get; set; }

    public int? NoCajaInterfazPaquete { get; set; }
}
