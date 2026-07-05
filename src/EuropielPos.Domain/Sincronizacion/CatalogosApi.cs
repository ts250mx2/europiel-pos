namespace EuropielPos.Domain.Sincronizacion;

// DTOs de los catálogos que publica el API central (port de las clases i_*
// de modInterfaz.vb). PascalCase salvo donde el API usa otra convención.

public class ValorSucursalesHorario
{
    public List<SucursalHorarioApi>? SucursalesHorario { get; set; }
}

public class SucursalHorarioApi
{
    public int IdDetalle { get; set; }

    public int IdSucursal { get; set; }

    public int Dia { get; set; }

    public DateTime Apertura { get; set; }

    public DateTime Cierre { get; set; }

    public bool BloqueoxComida { get; set; }

    public bool BloqueoxDescanso { get; set; }

    public DateTime AperturaRecepcion { get; set; }

    public DateTime CierreRecepcion { get; set; }

    public DateTime? FechaInicioEspecial { get; set; }

    public DateTime? FechaFinEspecial { get; set; }

    public bool BloqueoxComidaMediaHora { get; set; }
}

public class ValorSucursalesBono
{
    public List<SucursalBonoApi>? SucursalesBono { get; set; }
}

public class SucursalBonoApi
{
    public int IdBono { get; set; }

    public int IdSucursal { get; set; }

    public string? TipoBono { get; set; }

    public decimal Venta { get; set; }

    public decimal Premio { get; set; }

    public string? Tipo { get; set; }
}

public class ValorServicios
{
    public List<ServicioApi>? Servicios { get; set; }
}

public class ServicioApi
{
    public int IdServicio { get; set; }

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public int Orden { get; set; }

    public int IdTipoCita { get; set; }

    public int IdTipoServicio { get; set; }

    public int SesionesPaquete { get; set; }

    public decimal SesionesPaquetePrecio { get; set; }

    public int Duracion { get; set; }

    public bool PermitirVenta { get; set; }

    public int TrDuracionServicio { get; set; }

    public int TrDuracionLaser { get; set; }

    public int MaxVentasPorCliente { get; set; }
}

public class ValorUsuarios
{
    public List<UsuarioApi>? Usuarios { get; set; }
}

public class UsuarioApi
{
    public int IdUsuario { get; set; }

    public int IdSucursal { get; set; }

    public string? Nombre { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? TipoUsuario { get; set; }

    public bool EsActivo { get; set; }

    public int IdSucursalAgendaExterna { get; set; }

    public int EsUsuarioSistema { get; set; }

    public int UsuarioInasistencia { get; set; }

    public string? MsjUsuarioInasistencia { get; set; }
}

public class ValorHorariosCierreJuntas
{
    public List<HorarioCierreJuntasApi>? HorariosCierreJuntas { get; set; }
}

/// <summary>
/// El API devuelve este payload con claves snake_case porque el VB
/// deserializaba directo a la entidad EF <c>mobile_horarios_cierre_juntas_paso</c>.
/// </summary>
#pragma warning disable IDE1006
public class HorarioCierreJuntasApi
{
    public int id { get; set; }

    public int? id_sucursal { get; set; }

    public DateTime? fecha { get; set; }

    public DateTime? fecha_fin { get; set; }
}
#pragma warning restore IDE1006

public class ValorPagosPaquete
{
    public List<ConfigPagosPaqueteApi>? PagosPaquete { get; set; }
}

public class ConfigPagosPaqueteApi
{
    public int IdDetalle { get; set; }

    public int NoServiciosIni { get; set; }

    public int NoServiciosFin { get; set; }

    public int MaxPagos { get; set; }

    public int DiasMaximoPagos { get; set; }

    public int DiasMaximoDifEntrePagos { get; set; }

    public string? Tipo { get; set; }

    public decimal VentaMinima { get; set; }
}

public class ValorPrecioServicio
{
    public List<ConfigPrecioServicioApi>? PrecioServicio { get; set; }
}

public class ConfigPrecioServicioApi
{
    public int IdDetalle { get; set; }

    public int Orden { get; set; }

    public string? TipoDescuento { get; set; }

    public decimal Descuento { get; set; }

    public string? Tipo { get; set; }

    public int NoServiciosIni { get; set; }

    public int NoServiciosFin { get; set; }
}

public class ValorTipoIdentificacion
{
    public List<TipoIdentificacionApi>? TipoIdentificacion { get; set; }
}

/// <summary>Este catálogo viaja en snake_case/minúsculas, como el i_* original.</summary>
#pragma warning disable IDE1006
public class TipoIdentificacionApi
{
    public int id_tipo_identificacion { get; set; }

    public string? clave { get; set; }

    public string? descripcion { get; set; }
}
#pragma warning restore IDE1006

public class ValorServicioCombo
{
    public List<ServicioComboApi>? ServicioCombo { get; set; }
}

public class ServicioComboApi
{
    public int IdDetalle { get; set; }

    public string? Abreviatura { get; set; }

    public string? Descripcion { get; set; }

    public decimal Monto { get; set; }

    public string? IdsServicio { get; set; }
}

/// <summary>Respuesta de GetServerDate (fecha del servidor central).</summary>
#pragma warning disable IDE1006
public class FechaServidorRespuesta
{
    public string? Message { get; set; }

    public DateTime fecha_servidor { get; set; }
}

public class FechaServidorDetalleRespuesta
{
    public string? Message { get; set; }

    public DateTime fecha_servidor { get; set; }

    public long epoch { get; set; }
}
#pragma warning restore IDE1006
