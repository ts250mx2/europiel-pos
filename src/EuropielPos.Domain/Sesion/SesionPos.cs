namespace EuropielPos.Domain.Sesion;

/// <summary>
/// Sesión del usuario autenticado en esta caja (los datos que el original
/// guardaba en modGeneral tras el login). Singleton en el contenedor.
/// </summary>
public class SesionPos
{
    public bool Autenticado { get; set; }

    public int IdUsuario { get; set; }

    public string UsuarioNombre { get; set; } = string.Empty;

    public string UsuarioLogin { get; set; } = string.Empty;

    public string TipoUsuario { get; set; } = string.Empty;

    public int? IdSucursalAgendaExterna { get; set; }

    public string NombreSucursal { get; set; } = string.Empty;

    public string CiudadSucursal { get; set; } = string.Empty;

    public void Cerrar()
    {
        Autenticado = false;
        IdUsuario = 0;
        UsuarioNombre = string.Empty;
        UsuarioLogin = string.Empty;
        TipoUsuario = string.Empty;
        IdSucursalAgendaExterna = null;
    }
}
