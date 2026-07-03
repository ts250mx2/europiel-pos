using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class VersionesInstaladas
{
    public int Id { get; set; }

    public string? Version { get; set; }

    public DateTime? Fecha { get; set; }
}
