using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public string? Nombre { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? TipoUsuario { get; set; }

    public int? IdSucursal { get; set; }

    public bool? EsActivo { get; set; }

    public string? Email { get; set; }

    public int? IdSucursalAgendaExterna { get; set; }

    public int? EsUsuarioSistema { get; set; }

    public int? IdUsuarioSlack { get; set; }

    public int? UsuarioInasistencia { get; set; }

    public string? MsjUsuarioInasistencia { get; set; }
}
