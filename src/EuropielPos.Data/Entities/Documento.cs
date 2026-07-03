using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Documento
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdDocumento { get; set; }

    public int? IdTipoDocumento { get; set; }

    public int? IdRelacion { get; set; }

    public int? IdUsuarioAlta { get; set; }

    public DateTime? FechaAlta { get; set; }

    public string? Nombre { get; set; }

    public string? Referencia { get; set; }

    public int? IdSucursal { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual TipoDocumento? IdTipoDocumentoNavigation { get; set; }
}
