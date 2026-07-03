using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class NetpayUrl
{
    public string? Url { get; set; }

    public int? EsBorrada { get; set; }

    public int? EsDefault { get; set; }

    public int Id { get; set; }

    public int? UsarTls12 { get; set; }
}
