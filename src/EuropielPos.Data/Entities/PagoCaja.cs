using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class PagoCaja
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdPago { get; set; }

    public DateTime? Fecha { get; set; }

    public int? IdUsuario { get; set; }

    public string? TipoRecibo { get; set; }

    public string? Nombre { get; set; }

    public string? Domicilio { get; set; }

    public string? Rfc { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? Iva { get; set; }

    public decimal? Total { get; set; }

    public decimal? Pago { get; set; }

    public int? FolioRecibo { get; set; }

    public int? IdPacienteLocal { get; set; }

    public int? IdPaciente { get; set; }

    public DateTime? FechaAlta { get; set; }

    public int? IdBanco { get; set; }

    public int? IdSucursal { get; set; }

    public int? EsAnticipo { get; set; }

    public string? FolioFacturacion { get; set; }

    public int? EsEuroskin { get; set; }

    public virtual ICollection<PagoCajaDetalle> PagoCajaDetalle { get; set; } = new List<PagoCajaDetalle>();

    public virtual ICollection<PagoCajaForma> PagoCajaForma { get; set; } = new List<PagoCajaForma>();
}
