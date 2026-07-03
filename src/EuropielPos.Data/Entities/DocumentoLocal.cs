using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class DocumentoLocal
{
    public int Id { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdTipoDocumento { get; set; }

    public int? IdRelacion { get; set; }

    public int? IdRelacionLocal { get; set; }

    public int? IdUsuarioAlta { get; set; }

    public DateTime? FechaAlta { get; set; }

    public string? NombreArchivo { get; set; }

    public string? Referencia { get; set; }

    public int? IdSucursalInterfaz { get; set; }

    public int? NoCajaInterfaz { get; set; }
}
