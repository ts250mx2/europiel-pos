using EuropielPos.Data.Entities;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Bloque 2b del port de <c>modInterfaz.vb</c>: descarga de catálogos del
/// servidor central hacia la BD local (horarios, bonos, servicios, usuarios,
/// configuraciones de pago/precio, tipos de identificación, combos y fecha
/// del servidor).
/// </summary>
public interface IInterfazCatalogosDescargaService
{
    Task<string> GetSucursalesHorarioAsync(int claveBloque, CancellationToken ct = default);

    Task<string> GetSucursalesBonoAsync(int claveBloque, CancellationToken ct = default);

    Task<string> GetServiciosAsync(int claveBloque, CancellationToken ct = default);

    Task<string> GetUsuariosAsync(int claveBloque, CancellationToken ct = default);

    Task<DateTime> GetServerDateAsync(int claveBloque, CancellationToken ct = default);

    Task<FechaServidorDetalleRespuesta?> GetServerDateDetailAsync(int claveBloque, CancellationToken ct = default);

    Task<string> GetHorariosCierreJuntasAsync(int claveBloque, CancellationToken ct = default);

    Task<string> GetConfigPagosPaqueteAsync(int claveBloque, CancellationToken ct = default);

    Task<string> GetConfigPrecioServicioAsync(int claveBloque, CancellationToken ct = default);

    Task<string> GetTipoIdentificacionAsync(int claveBloque, CancellationToken ct = default);

    Task<string> GetServicioComboAsync(int claveBloque, CancellationToken ct = default);
}

public class InterfazCatalogosDescargaService : IInterfazCatalogosDescargaService
{
    private readonly PosDbContext _db;
    private readonly IClienteApiPos _api;
    private readonly ContextoPos _contexto;

    public InterfazCatalogosDescargaService(PosDbContext db, IClienteApiPos api, ContextoPos contexto)
    {
        _db = db;
        _api = api;
        _contexto = contexto;
    }

    /// <remarks>Reemplazo total: borra los horarios locales y carga los del servidor.</remarks>
    public async Task<string> GetSucursalesHorarioAsync(int claveBloque, CancellationToken ct = default)
    {
        bool existenSucursales = await _db.Sucursal.AnyAsync(x => x.EsActiva == 1, ct);

        try
        {
            var respuesta = await _api.PostAsync<RespuestaApi<ValorSucursalesHorario>>(
                "/api/europielpos/GetSucursalesHorario", headersExtra: HeaderBloque(claveBloque), ct: ct);

            var horarios = respuesta?.Value?.SucursalesHorario;
            if (horarios is null || horarios.Count == 0)
                return "OK";

            await _db.SucursalHorario.Where(x => x.IdDetalle > 0).ExecuteDeleteAsync(ct);

            foreach (var s in horarios)
            {
                _db.SucursalHorario.Add(new SucursalHorario
                {
                    IdDetalle = s.IdDetalle,
                    IdSucursal = s.IdSucursal,
                    Dia = s.Dia,
                    Apertura = s.Apertura,
                    Cierre = s.Cierre,
                    BloqueoXComida = s.BloqueoxComida,
                    BloqueoXDescanso = s.BloqueoxDescanso,
                    AperturaRecepcion = s.AperturaRecepcion,
                    CierreRecepcion = s.CierreRecepcion,
                    FechaInicioEspecial = s.FechaInicioEspecial,
                    FechaFinEspecial = s.FechaFinEspecial,
                    BloqueoXComidaMediaHora = s.BloqueoxComidaMediaHora,
                });
            }

            await _db.SaveChangesAsync(ct);
            return "OK";
        }
        catch (Exception ex)
        {
            throw ErrorDescarga("HttpPost_GetSucursalesHorario", ex, existenSucursales);
        }
    }

    /// <remarks>Reemplazo total de los bonos por sucursal.</remarks>
    public async Task<string> GetSucursalesBonoAsync(int claveBloque, CancellationToken ct = default)
    {
        bool existenSucursales = await _db.Sucursal.AnyAsync(x => x.EsActiva == 1, ct);

        try
        {
            var respuesta = await _api.PostAsync<RespuestaApi<ValorSucursalesBono>>(
                "/api/europielpos/GetSucursalesBono", headersExtra: HeaderBloque(claveBloque), ct: ct);

            var bonos = respuesta?.Value?.SucursalesBono;
            if (bonos is null || bonos.Count == 0)
                return "OK";

            await _db.BonosPorSucursal.Where(x => x.IdBono > 0).ExecuteDeleteAsync(ct);

            foreach (var b in bonos)
            {
                _db.BonosPorSucursal.Add(new BonosPorSucursal
                {
                    IdBono = b.IdBono,
                    IdSucursal = b.IdSucursal,
                    TipoBono = b.TipoBono,
                    Venta = b.Venta,
                    Premio = b.Premio,
                    Tipo = b.Tipo,
                });
            }

            await _db.SaveChangesAsync(ct);
            return "OK";
        }
        catch (Exception ex)
        {
            throw ErrorDescarga("HttpPost_GetSucursalesBono", ex, existenSucursales);
        }
    }

    /// <remarks>Upsert del catálogo de servicios de la sucursal configurada.</remarks>
    public async Task<string> GetServiciosAsync(int claveBloque, CancellationToken ct = default)
    {
        try
        {
            string cuerpo = await CuerpoIdSucursalAsync(ct);

            var respuesta = await _api.PostAsync<RespuestaApi<ValorServicios>>(
                "/api/europielpos/GetServicios", cuerpo, HeaderBloque(claveBloque), ct);

            var servicios = respuesta?.Value?.Servicios;
            if (servicios is null || servicios.Count == 0)
                return "OK";

            DateTime fechaHoy = DateTime.Now;

            foreach (var s in servicios)
            {
                var servicio = await _db.Servicio.FirstOrDefaultAsync(x => x.IdServicio == s.IdServicio, ct);

                if (servicio is null)
                {
                    servicio = new Servicio { IdServicio = s.IdServicio };
                    _db.Servicio.Add(servicio);
                }

                servicio.FechaInterfaz = fechaHoy;
                servicio.Descripcion = s.Descripcion;
                servicio.Precio = s.Precio;
                servicio.Orden = s.Orden;
                // El original asignaba las navegaciones tipo_cita/tipo_servicio;
                // el efecto neto es fijar la FK si el catálogo local la tiene.
                servicio.IdTipoCita = await _db.TipoCita.AnyAsync(x => x.IdTipoCita == s.IdTipoCita, ct) ? s.IdTipoCita : null;
                servicio.IdTipoServicio = await _db.TipoServicio.AnyAsync(x => x.IdTipoServicio == s.IdTipoServicio, ct) ? s.IdTipoServicio : null;
                servicio.SesionesPaquete = s.SesionesPaquete;
                servicio.SesionesPaquetePrecio = s.SesionesPaquetePrecio;
                servicio.Duracion = s.Duracion;
                servicio.PermitirVenta = s.PermitirVenta;
                servicio.TrDuracionServicio = s.TrDuracionServicio;
                servicio.TrDuracionLaser = s.TrDuracionLaser;
                servicio.MaxVentasPorCliente = s.MaxVentasPorCliente;

                await _db.SaveChangesAsync(ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetServicios: " + ex.Message, ex);
        }
    }

    /// <remarks>Reemplazo total: borra los usuarios locales y carga los del servidor.</remarks>
    public async Task<string> GetUsuariosAsync(int claveBloque, CancellationToken ct = default)
    {
        try
        {
            string cuerpo = await CuerpoIdSucursalAsync(ct);

            var respuesta = await _api.PostAsync<RespuestaApi<ValorUsuarios>>(
                "/api/europielpos/GetUsuarios", cuerpo, HeaderBloque(claveBloque), ct);

            var usuarios = respuesta?.Value?.Usuarios;
            if (usuarios is null || usuarios.Count == 0)
                return "OK";

            DateTime fechaHoy = DateTime.Now;

            await _db.Usuario.Where(x => x.IdUsuario > 0).ExecuteDeleteAsync(ct);

            foreach (var u in usuarios)
            {
                _db.Usuario.Add(new Usuario
                {
                    IdUsuario = u.IdUsuario,
                    IdSucursal = u.IdSucursal,
                    Login = u.Login,
                    Nombre = u.Nombre,
                    Password = u.Password,
                    TipoUsuario = u.TipoUsuario,
                    EsActivo = u.EsActivo,
                    IdSucursalAgendaExterna = u.IdSucursalAgendaExterna,
                    FechaInterfaz = fechaHoy,
                    EsUsuarioSistema = u.EsUsuarioSistema,
                    UsuarioInasistencia = u.UsuarioInasistencia,
                    MsjUsuarioInasistencia = u.MsjUsuarioInasistencia,
                });
            }

            await _db.SaveChangesAsync(ct);
            return "OK";
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetUsuarios: " + ex.Message, ex);
        }
    }

    /// <remarks>Fecha del servidor central; si no responde, el original
    /// devolvía la hora local menos 7 minutos como aproximación.</remarks>
    public async Task<DateTime> GetServerDateAsync(int claveBloque, CancellationToken ct = default)
    {
        try
        {
            string cuerpo = await CuerpoIdSucursalAsync(ct);

            var respuesta = await _api.PostAsync<FechaServidorRespuesta>(
                "/api/europielpos/GetServerDate", cuerpo, HeaderBloque(claveBloque), ct);

            if (respuesta is not null && !string.IsNullOrEmpty(respuesta.Message))
                return respuesta.fecha_servidor;

            return DateTime.Now.AddMinutes(-7.0);
        }
        catch
        {
            return DateTime.Now.AddMinutes(-7.0);
        }
    }

    public async Task<FechaServidorDetalleRespuesta?> GetServerDateDetailAsync(int claveBloque, CancellationToken ct = default)
    {
        try
        {
            string cuerpo = await CuerpoIdSucursalAsync(ct);

            return await _api.PostAsync<FechaServidorDetalleRespuesta>(
                "/api/europielpos/GetServerDateDetail", cuerpo, HeaderBloque(claveBloque), ct);
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetServerDateDetail: " + ex.Message, ex);
        }
    }

    /// <remarks>Inserta los horarios en la tabla de paso y ejecuta el SP que
    /// los aplica (el original usaba SqlBulkCopy + CitaBL.ProcesaHorariosCierreJuntas).</remarks>
    public async Task<string> GetHorariosCierreJuntasAsync(int claveBloque, CancellationToken ct = default)
    {
        try
        {
            string cuerpo = await CuerpoIdSucursalAsync(ct);

            var respuesta = await _api.PostAsync<RespuestaApi<ValorHorariosCierreJuntas>>(
                "/api/europielpos/GetHorariosCierreJuntas", cuerpo, HeaderBloque(claveBloque), ct);

            var horarios = respuesta?.Value?.HorariosCierreJuntas;
            if (horarios is null || horarios.Count == 0)
                return "OK";

            foreach (var h in horarios)
            {
                _db.MobileHorariosCierreJuntasPaso.Add(new MobileHorariosCierreJuntasPaso
                {
                    IdSucursal = h.id_sucursal,
                    Fecha = h.fecha,
                    FechaFin = h.fecha_fin,
                });
            }

            await _db.SaveChangesAsync(ct);
            await _db.Database.ExecuteSqlAsync($"EXEC procesa_mobile_horarios_cierre_juntas_paso", ct);

            return "OK";
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetHorariosCierreJuntas: " + ex.Message, ex);
        }
    }

    public async Task<string> GetConfigPagosPaqueteAsync(int claveBloque, CancellationToken ct = default)
    {
        bool existenSucursales = await _db.Sucursal.AnyAsync(x => x.EsActiva == 1, ct);

        try
        {
            var respuesta = await _api.PostAsync<RespuestaApi<ValorPagosPaquete>>(
                "/api/europielpos/GetConfigPagosPaquete", headersExtra: HeaderBloque(claveBloque), ct: ct);

            var configs = respuesta?.Value?.PagosPaquete;
            if (configs is null || configs.Count == 0)
                return "OK";

            foreach (var c in configs)
            {
                var config = await _db.ConfigPagosPaquete.FirstOrDefaultAsync(x => x.IdDetalle == c.IdDetalle, ct);

                if (config is null)
                {
                    config = new ConfigPagosPaquete { IdDetalle = c.IdDetalle };
                    _db.ConfigPagosPaquete.Add(config);
                }

                config.Tipo = c.Tipo;
                config.NoServiciosIni = c.NoServiciosIni;
                config.NoServiciosFin = c.NoServiciosFin;
                config.MaxPagos = c.MaxPagos;
                config.DiasMaximoPagos = c.DiasMaximoPagos;
                config.DiasMaximoDifEntrePagos = c.DiasMaximoDifEntrePagos;
                config.VentaMinima = c.VentaMinima;

                await _db.SaveChangesAsync(ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            throw ErrorDescarga("HttpPost_GetConfigPagosPaquete", ex, existenSucursales);
        }
    }

    public async Task<string> GetConfigPrecioServicioAsync(int claveBloque, CancellationToken ct = default)
    {
        bool existenSucursales = await _db.Sucursal.AnyAsync(x => x.EsActiva == 1, ct);

        try
        {
            string cuerpo = $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\" }}";

            var respuesta = await _api.PostAsync<RespuestaApi<ValorPrecioServicio>>(
                "/api/europielpos/GetConfigPrecioServicio", cuerpo, HeaderBloque(claveBloque), ct);

            var configs = respuesta?.Value?.PrecioServicio;
            if (configs is null || configs.Count == 0)
                return "OK";

            foreach (var c in configs)
            {
                var config = await _db.ConfigPrecioServicio.FirstOrDefaultAsync(x => x.IdDetalle == c.IdDetalle, ct);

                if (config is null)
                {
                    config = new ConfigPrecioServicio { IdDetalle = c.IdDetalle };
                    _db.ConfigPrecioServicio.Add(config);
                }

                config.Orden = c.Orden;
                config.TipoDescuento = c.TipoDescuento;
                config.Descuento = c.Descuento;
                config.Tipo = c.Tipo;
                config.NoServiciosIni = c.NoServiciosIni;
                config.NoServiciosFin = c.NoServiciosFin;

                await _db.SaveChangesAsync(ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            throw ErrorDescarga("HttpPost_GetConfigPrecioServicio", ex, existenSucursales);
        }
    }

    public async Task<string> GetTipoIdentificacionAsync(int claveBloque, CancellationToken ct = default)
    {
        bool existenSucursales = await _db.Sucursal.AnyAsync(x => x.EsActiva == 1, ct);

        try
        {
            var respuesta = await _api.PostAsync<RespuestaApi<ValorTipoIdentificacion>>(
                "/api/europielpos/GetTipoIdentificacion", headersExtra: HeaderBloque(claveBloque), ct: ct);

            var tipos = respuesta?.Value?.TipoIdentificacion;
            if (tipos is null || tipos.Count == 0)
                return "OK";

            foreach (var t in tipos)
            {
                var tipo = await _db.TipoIdentificacion.FirstOrDefaultAsync(x => x.IdTipoIdentificacion == t.id_tipo_identificacion, ct);

                if (tipo is null)
                {
                    tipo = new TipoIdentificacion { IdTipoIdentificacion = t.id_tipo_identificacion };
                    _db.TipoIdentificacion.Add(tipo);
                }

                tipo.Clave = t.clave;
                tipo.Descripcion = t.descripcion;

                await _db.SaveChangesAsync(ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            throw ErrorDescarga("HttpPost_GetTipoIdentificacion", ex, existenSucursales);
        }
    }

    public async Task<string> GetServicioComboAsync(int claveBloque, CancellationToken ct = default)
    {
        bool existenSucursales = await _db.Sucursal.AnyAsync(x => x.EsActiva == 1, ct);

        try
        {
            var respuesta = await _api.PostAsync<RespuestaApi<ValorServicioCombo>>(
                "/api/europielpos/GetServicioCombo", headersExtra: HeaderBloque(claveBloque), ct: ct);

            var combos = respuesta?.Value?.ServicioCombo;
            if (combos is null || combos.Count == 0)
                return "OK";

            foreach (var c in combos)
            {
                var combo = await _db.ServicioCombo.FirstOrDefaultAsync(x => x.IdDetalle == c.IdDetalle, ct);

                if (combo is null)
                {
                    combo = new ServicioCombo { IdDetalle = c.IdDetalle };
                    _db.ServicioCombo.Add(combo);
                }

                combo.Abreviatura = c.Abreviatura;
                combo.Descripcion = c.Descripcion;
                combo.Monto = c.Monto;
                combo.IdsServicio = c.IdsServicio;

                await _db.SaveChangesAsync(ct);
            }

            return "OK";
        }
        catch (Exception ex)
        {
            throw ErrorDescarga("HttpPost_GetServicioCombo", ex, existenSucursales);
        }
    }

    // ----- Helpers -----

    private static Dictionary<string, string> HeaderBloque(int claveBloque) =>
        new() { ["ClaveBloque"] = claveBloque.ToString() };

    /// <summary>Cuerpo <c>{ "IdSucursal": "..." }</c> leído de la tabla parametro,
    /// como hacía el original.</summary>
    private async Task<string> CuerpoIdSucursalAsync(CancellationToken ct)
    {
        var parametro = await _db.Parametro.AsNoTracking().FirstOrDefaultAsync(ct);
        return $"{{ \"IdSucursal\": \"{parametro?.IdSucursal}\" }}";
    }

    /// <summary>Replica el manejo de error del original: si la caja aún no
    /// tiene sucursales, el problema casi siempre es conectividad.</summary>
    private static Exception ErrorDescarga(string metodo, Exception ex, bool existenSucursales)
    {
        string mensaje = existenSucursales ? ex.Message : "Verifique su conexion a internet.";
        return new Exception($"Error {metodo}: {mensaje}", ex);
    }
}
