using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class LogEnvioReconocimientoFacial
{
    public int Id { get; set; }

    public int? BranchId { get; set; }

    public string? Tipo { get; set; }

    public string? IdFeedBack { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? IdUsuarioRegistro { get; set; }

    public int? IdGio { get; set; }
}
