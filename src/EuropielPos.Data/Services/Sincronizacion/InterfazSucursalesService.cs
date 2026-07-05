using EuropielPos.Data.Entities;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Port de <c>HttpPost_GetSucursales</c> y <c>HttpPost_SendMessageToSlack</c>
/// de modInterfaz.vb: descarga el catálogo de sucursales, actualiza la
/// configuración local (<c>parametro</c>) y notifica a Slack.
/// </summary>
public interface IInterfazSucursalesService
{
    Task<string> GetSucursalesAsync(int claveBloque, int idBloque, CancellationToken ct = default);

    Task<string> SendMessageToSlackAsync(string messageCode, string emitter, string countryCode, string forceMsg, CancellationToken ct = default);
}

public class InterfazSucursalesService : IInterfazSucursalesService
{
    private readonly PosDbContext _db;
    private readonly IClienteApiPos _api;
    private readonly ContextoPos _contexto;

    public InterfazSucursalesService(PosDbContext db, IClienteApiPos api, ContextoPos contexto)
    {
        _db = db;
        _api = api;
        _contexto = contexto;
    }

    public async Task<string> GetSucursalesAsync(int claveBloque, int idBloque, CancellationToken ct = default)
    {
        bool existenSucursales = await _db.Sucursal.AnyAsync(x => x.EsActiva == 1, ct);

        try
        {
            // La caja ya configurada manda su identidad; una caja nueva manda cuerpo vacío.
            string cuerpo = _contexto.NoCaja is null
                ? string.Empty
                : $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\", \"NoCaja\": \"{_contexto.NoCaja}\", " +
                  $"\"VersionLocal\": \"{_contexto.VersionSistema}\", \"EsEuroskin\": \"{_contexto.EsEuroskin}\"}}";

            var respuesta = await _api.PostAsync<RespuestaApi<ValorSucursales>>(
                "/api/europielpos/GetSucursales", cuerpo,
                new Dictionary<string, string>
                {
                    ["ClaveBloque"] = claveBloque.ToString(),
                    ["IdBloque"] = idBloque.ToString(),
                }, ct);

            var sucursalesApi = respuesta?.Value?.Sucursales;
            if (sucursalesApi is null || sucursalesApi.Count == 0)
                return "OK";

            DateTime fechaHoy = DateTime.Now;

            await ActualizaParametroAsync(sucursalesApi, ct);

            // Upsert de cada sucursal del catálogo.
            foreach (var s in sucursalesApi)
            {
                var sucursal = await _db.Sucursal.FirstOrDefaultAsync(x => x.IdSucursal == s.IdSucursal, ct);

                if (sucursal is null)
                {
                    sucursal = new Sucursal { IdSucursal = s.IdSucursal };
                    _db.Sucursal.Add(sucursal);
                }

                AplicaCamposSucursal(sucursal, s, fechaHoy);
                await _db.SaveChangesAsync(ct);
            }

            // Las sucursales locales que ya no vienen del servidor se desactivan.
            var sucursalesActivas = await _db.Sucursal.Where(x => x.EsActiva == 1).ToListAsync(ct);
            foreach (var local in sucursalesActivas)
            {
                if (!sucursalesApi.Any(x => x.IdSucursal == local.IdSucursal))
                {
                    local.EsActiva = 0;
                    await _db.SaveChangesAsync(ct);
                }
            }

            // El procesador de cobros default se toma de la sucursal configurada.
            var parametro = await _db.Parametro.FirstOrDefaultAsync(ct);
            if (parametro?.IdSucursal is not null)
            {
                var sucursalDefault = sucursalesApi.FirstOrDefault(x => x.IdSucursal == parametro.IdSucursal);
                if (sucursalDefault is not null)
                {
                    parametro.PosProcesadorCobrosDefault = sucursalDefault.PosProcesadorCobrosDefault;
                    await _db.SaveChangesAsync(ct);
                }
            }

            return "OK";
        }
        catch (Exception ex)
        {
            // El original registraba en log_errores vía manejo_errores.GuardaLogError
            // y ajustaba el mensaje si era una caja sin sucursales (sin internet).
            string mensaje = existenSucursales ? ex.Message : "Verifique su conexion a internet.";
            throw new Exception("Error HttpPost_GetSucursales: " + mensaje, ex);
        }
    }

    /// <summary>
    /// Copia a <c>parametro</c> la configuración de la sucursal seleccionada
    /// (la del <c>ContextoPos</c>), creando el registro si no existe.
    /// </summary>
    private async Task ActualizaParametroAsync(List<SucursalApi> sucursales, CancellationToken ct)
    {
        var par = sucursales.FirstOrDefault(x => x.IdSucursal == _contexto.IdSucursal);
        if (par is null)
            return;

        var parametro = await _db.Parametro.FirstOrDefaultAsync(ct);
        if (parametro is null)
        {
            parametro = new Parametro();
            _db.Parametro.Add(parametro);
        }

        parametro.Idioma = par.Idioma;
        parametro.MaxPagosVendedor = par.MaxPagosVendedor;
        parametro.DiasMaximoPagos = par.DiasMaximoPagos;
        parametro.AnticipoMinimo = par.AnticipoMinimo;
        parametro.AnticipoMinimoCauto = par.AnticipoMinimoCauto;
        parametro.AnticipoMinimoReventa = par.AnticipoMinimoReventa;
        parametro.NombreEmpresa = par.NombreEmpresa;
        parametro.RepresentanteLegal = par.RepresentanteLegal;
        parametro.CcRepresentanteLegal = par.CCRepresentanteLegal;
        parametro.Moneda = par.Moneda;
        parametro.PagosDefault = par.PagosDefault;
        parametro.TipoDifEntrePagos = par.TipoDifEntrePagos;
        parametro.CapturaNoCuentaActivo = par.CapturaNoCuentaActivo;
        parametro.CapturaIbanActivo = par.CapturaIbanActivo;
        parametro.HorasParaReventa = par.HorasParaReventa;
        parametro.CapturaDniActivo = par.CapturaDniActivo;
        parametro.MontoProtegido = par.MontoProtegido;
        parametro.MontoMaxPorcProtegido = par.MontoMaxPorcProtegido;
        parametro.PorcParaProtegido = par.PorcParaProtegido;
        parametro.RazonSocial = par.RazonSocial;
        parametro.Rfc = par.Rfc;
        parametro.DiasMaximo1erPago = par.DiasMaximo1erPago;
        parametro.DiasMaximoDifEntrePagos = par.DiasMaximoDifEntrePagos;
        parametro.ValidarTarjetaBinNoPermitidoPos = par.ValidarTarjetaBinNoPermitidoPos;
        parametro.ReciboLeyenda1 = par.ReciboLeyenda1;
        parametro.ReciboLeyenda2 = par.ReciboLeyenda2;
        parametro.PosProcesadorCobrosDefault = par.PosProcesadorCobrosDefault;
        parametro.DiasMaximoPagosDir = par.DiasMaximosPagosDir;
        parametro.DiasMaximoPagosContraloria = par.DiasMaximosPagosContraloria;
        parametro.MaxPagosVerificador = par.MaxPagosVerificador;
        parametro.MaxPagosV1 = par.MaxPagosV1;
        parametro.MaxPagosV2 = par.MaxPagosV2;
        parametro.DiasMaximoPagosV1 = par.DiasMaximoPagoV1;
        parametro.DiasMaximoPagosV2 = par.DiasMaximoPagoV2;
        parametro.EmailsNominaEnfermera = par.EmailsNominaEnfermera;
        parametro.CapturaIbanErroneo = par.CapturaIbanErroneo;
        parametro.PermitirEnvioSmsAppWeb = par.PermitirEnvioSMSAppWeb;
        parametro.AnticipoMinimoSinIban = par.AnticipoMininoSinIBAN;
        parametro.PermitirCitaPaqueteConNegrita = par.PermitirCitaPaqueteConNegrita;
        parametro.UsarConfiguracionDePagosBancomer = par.UsarConfiguracionDePagosBancomer;
        parametro.Abreviatura = par.Abreviatura;
        parametro.VentaMinimaPaquete = par.VentaMinimaPaquete;
        parametro.ReventaMinimaPaquete = par.ReventaMinimaPaquete;
        parametro.MontoMaximoCobro = par.MontoMaximoCobro;
        parametro.NoSesionesDefault = Convert.ToInt32(par.NoSesionesDefault); // la columna local es int; VB convertía implícito
        parametro.Nit = par.Nit;
        parametro.EsEuroskin = par.EsEuroskin;
        parametro.BloqueoPos = par.BloqueoPos;
        parametro.BloqueoPosAgenda = par.BloqueoPosAgenda;
        parametro.EsCredibanco = par.EsCredibanco;
        parametro.EsEfevoo = par.EsEfevoo;
        parametro.EfevooDeviceId = par.EfevooDeviceId;
        parametro.EfevooSecret = par.EfevooSecret;
        parametro.ValidaToku = par.ValidaToku;
        parametro.TerminalIdMercadoPago = par.TerminalIdMercadoPago;

        await _db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Copia los campos del API a la entidad local. En el VB este bloque
    /// estaba duplicado para insert y update; aquí es uno solo.
    /// </summary>
    private static void AplicaCamposSucursal(Sucursal sucursal, SucursalApi s, DateTime fechaInterfaz)
    {
        sucursal.IdPais = s.IdPais;
        sucursal.FechaInterfaz = fechaInterfaz;
        sucursal.Descripcion = s.Descripcion;
        sucursal.Prefijo = s.Prefijo;
        sucursal.Timezone = s.TimeZone;
        sucursal.Direccion = s.Direccion;
        sucursal.StoreidTerminalInterfaz = s.StoreidTerminalInterfaz;
        sucursal.UsernameTerminalInterfaz = s.UsernameTerminalInterfaz;
        sucursal.PasswordTerminalInterfaz = s.PasswordTerminalInterfaz;
        sucursal.CantidadCitasEmpalmadas = s.CantidadCitasEmpalmadas;
        sucursal.HorarioDiferenteRecepcion = s.HorarioDiferenteRecepcion;
        sucursal.IdBloque = s.IdBloque;
        sucursal.TipoTiempos = s.TipoTiempos;
        sucursal.DirectorComercial = s.DirectorComercial;
        sucursal.EmailDirectorComercial = s.EmailDirectorComercial;
        sucursal.Subdirector = s.Subdirector;
        sucursal.EmailSubdirector = s.EmailSubdirector;
        sucursal.DirectorComercialIdUsuarioSlack = s.DirectorComercialIdUsuarioSlack;
        sucursal.SubdirectorIdUsuarioSlack = s.SubdirectorIdUsuarioSlack;
        sucursal.EmailsNotificacionToken = s.EmailsNotificacionToken;
        sucursal.IdUsuariosSlackToken = s.IdUsuariosSlackToken;
        sucursal.VentaMinimaBono = s.VentaMinimaBono;
        sucursal.PorcentajeComisionesMenor = s.PorcentajeComisionesMenor;
        sucursal.PorcentajeComisionesMayor = s.PorcentajeComisionesMayor;
        sucursal.GteCuotaSemanalVenta = s.GteCuotaSemanalVenta;
        sucursal.GteCobranzaMinima = s.GteCobranzaMinima;
        sucursal.VentaMiminaBonoLd = s.VentaMiminaBonoLD;
        sucursal.MontoSobreventa = s.MontoSobreventa;
        sucursal.CuotaMinParaNoPerderBono = s.CuotaMinParaNoPerderBono;
        sucursal.CaidaViernes = s.CaidaViernes;
        sucursal.CaidaSabado = s.CaidaSabado;
        sucursal.CaidaDomingo = s.CaidaDomingo;
        sucursal.PotencialVenta = s.PotencialVenta;
        sucursal.Gerente = s.Gerente;
        sucursal.GioInicioTurno1 = s.GioInicioTurno1;
        sucursal.GioInicioTurno2 = s.GioInicioTurno2;
        sucursal.VendedorHoraCheckin = s.VendedorHoraCheckin;
        sucursal.HorarioEspecial = s.HorarioEspecial;
        sucursal.HorarioEspecialDiferenteRecepcion = s.HorarioEspecialDiferenteRecepcion;
        sucursal.CiudadSucursal = s.CiudadSucursal;
        sucursal.TiempoAdicionalPrimeraCita = s.TiempoAdicionalPrimeraCita;
        sucursal.TiempoMinimoPrimeraCita = s.TiempoMinimoPrimeraCita;
        sucursal.SesionExtendida = s.SesionExtendida;
        sucursal.AumentoSesionExtendida = s.AumentoSesionExtendida;
        sucursal.AumentoPorcSesionExtendida = s.AumentoPorcSesionExtendida;
        sucursal.TiempoMinimoSesionExtendida = s.TiempoMinimoSesionExtendida;
        sucursal.AplicaPreciosBajos = s.AplicaPreciosBajos;
        sucursal.CostoMaxVentaNuevaParaAplicarAnticipoPreciosBajos = s.CostoMaxVentaNuevaParaAplicarAnticipoPreciosBajos;
        sucursal.PacienteMinPagadoParaAnticipoReventaCero = s.PacienteMinPagadoParaAnticipoReventaCero;
        sucursal.PacienteMinPagadoHoyParaAnticipoReventaCero = s.PacienteMinPagadoHoyParaAnticipoReventaCero;
        sucursal.PermitirReventaSinAnticipoMinimo = s.PermitirReventaSinAnticipoMinimo;
        sucursal.EmailsTokenReventaEnCero = s.EmailsTokenReventaEnCero;
        sucursal.IdUsuariosSlackTokenReventaEnCero = s.IdUsuariosSlackTokenReventaEnCero;
        sucursal.ValidarAreasMaxVentasPorCliente = s.ValidarAreasMaxVentasPorCliente;
        sucursal.PermitirSolicitarHasta6pagosXToken = s.PermitirSolicitarHasta6pagosPorToken;
        sucursal.EmailsTokenPermitirSolicitarHasta6pagos = s.EmailsTokenPermitirSolicitarHasta6pagos;
        sucursal.IdUsuariosSlackTokenPermitirSolicitarHasta6pagos = s.IdUsuariosSlackTokenPermitirSolicitarHasta6pagos;
        sucursal.ValidarBinTarjetaAnticipoMinimo = s.ValidarBinTarjetaAnticipoMinimo;
        sucursal.ValidarBinTarjetaAnticipoMinimoMonto = s.ValidarBinTarjetaAnticipoMinimoMonto;
        sucursal.PermitirTokenizacionConekta = s.PermitirTokenizacionConekta;
        sucursal.UrlWebTokenizacionConekta = s.UrlWebTokenizacionConekta;
        sucursal.EsCredibanco = s.EsCredibanco;
        sucursal.EsEfevoo = s.EsEfevoo;
        sucursal.EfevooDeviceId = s.EfevooDeviceId;
        sucursal.EfevooSecret = s.EfevooSecret;
        sucursal.ValidaToku = s.ValidaToku;
        sucursal.TerminalIdMercadoPago = s.TerminalIdMercadoPago;
    }

    /// <summary>
    /// Notificación a Slack vía el API central. Igual que el original,
    /// normaliza acentos y eñes antes de enviar.
    /// </summary>
    public async Task<string> SendMessageToSlackAsync(string messageCode, string emitter, string countryCode, string forceMsg, CancellationToken ct = default)
    {
        try
        {
            forceMsg = QuitarAcentos(forceMsg);

            string cuerpo =
                $"{{ \"Bloque\": \"{_contexto.Bloque}\", \"IdSucursal\": \"{_contexto.IdSucursal}\", " +
                $"\"NoCaja\": \"{_contexto.NoCaja}\", \"MessageCode\": \"{messageCode}\", " +
                $"\"Emitter\": \"{emitter}\", \"CountryCode\": \"{countryCode}\", \"ForceMsg\": \"{forceMsg}\" }}";

            var respuesta = await _api.PostAsync<RespuestaSimple>(
                "/api/europielpos/SendMessageToSlack", cuerpo,
                new Dictionary<string, string> { ["ClaveBloque"] = _contexto.ClaveBloque }, ct);

            if (respuesta is not null && respuesta.Message != "ok")
                return "Error HttpPost_SendMessageToSlack: " + respuesta.Message;

            return "OK";
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_SendMessageToSlack: " + ex.Message, ex);
        }
    }

    private static string QuitarAcentos(string texto) => texto
        .Replace("Ñ", "N").Replace("ñ", "n")
        .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
        .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U");
}
