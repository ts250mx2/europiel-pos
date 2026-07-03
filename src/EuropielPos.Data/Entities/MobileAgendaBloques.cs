using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class MobileAgendaBloques
{
    public int Id { get; set; }

    public DateTime FechaInicio { get; set; }

    public string? Tipo { get; set; }
}
