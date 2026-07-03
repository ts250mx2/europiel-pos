using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ParametrosAnticipoPaso
{
    public int IdParametroDetalle { get; set; }

    public int IdParametro { get; set; }

    public int IdSucursal { get; set; }

    public string Bloque { get; set; } = null!;

    public int IdUsuarioAlta { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int IdTipo { get; set; }

    public string? Descripcion { get; set; }

    public decimal Valor { get; set; }

    public int TipoCalculo { get; set; }

    public decimal ValorMin { get; set; }

    public decimal ValorMax { get; set; }

    public int? IdBanco { get; set; }

    public bool EsReventa { get; set; }

    public int? DiaInicio { get; set; }

    public int? DiaFin { get; set; }

    public bool? Activo { get; set; }

    public bool? EsProtegido { get; set; }
}
