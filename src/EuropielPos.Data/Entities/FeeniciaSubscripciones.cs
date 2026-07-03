using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class FeeniciaSubscripciones
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int IdSubscripcion { get; set; }

    public string? MensajeRespuesta { get; set; }

    public string? CodigoRespuesta { get; set; }

    public string? IdPlan { get; set; }

    public int? PlanUnit { get; set; }

    public string? Payload { get; set; }

    public int IdLocalPaquete { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdBloque { get; set; }

    public bool Activo { get; set; }

    public virtual Paquete IdLocalPaqueteNavigation { get; set; } = null!;
}
