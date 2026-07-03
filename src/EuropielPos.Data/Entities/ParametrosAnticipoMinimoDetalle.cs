using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class ParametrosAnticipoMinimoDetalle
{
    public int IdParametroDetalle { get; set; }

    public int IdParametro { get; set; }

    public int IdTipo { get; set; }

    public decimal Valor { get; set; }

    public int TipoCalculo { get; set; }

    public decimal ValorMin { get; set; }

    public decimal ValorMax { get; set; }

    public int? IdBanco { get; set; }

    public bool EsReventa { get; set; }

    public int? DiaInicio { get; set; }

    public int? DiaFin { get; set; }

    public bool? EsProtegido { get; set; }

    public virtual ParametrosAnticipoMinimo IdParametroNavigation { get; set; } = null!;

    public virtual TipoParametroAnticipoMinimo IdTipoNavigation { get; set; } = null!;
}
