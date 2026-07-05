namespace EuropielPos.Domain.Sincronizacion;

public class ValorSucursales
{
    public List<SucursalApi>? Sucursales { get; set; }
}

/// <summary>
/// Sucursal como la publica el API central (port de <c>i_sucursal</c> de
/// modInterfaz.vb). Alimenta el upsert de las tablas <c>sucursal</c> y
/// <c>parametro</c> locales.
/// </summary>
public class SucursalApi
{
    public int IdSucursal { get; set; }

    public int IdPais { get; set; }

    public string? Descripcion { get; set; }

    public string? Prefijo { get; set; }

    public string? TimeZone { get; set; }

    public int MaxPagosVendedor { get; set; }

    public int DiasMaximoPagos { get; set; }

    public decimal AnticipoMinimo { get; set; }

    public decimal AnticipoMinimoCauto { get; set; }

    public decimal AnticipoMinimoReventa { get; set; }

    public string? Idioma { get; set; }

    public string? NombreEmpresa { get; set; }

    public string? RepresentanteLegal { get; set; }

    public string? CCRepresentanteLegal { get; set; }

    public string? Moneda { get; set; }

    public string? Direccion { get; set; }

    public string? StoreidTerminalInterfaz { get; set; }

    public string? UsernameTerminalInterfaz { get; set; }

    public string? PasswordTerminalInterfaz { get; set; }

    public int CantidadCitasEmpalmadas { get; set; }

    public int HorarioDiferenteRecepcion { get; set; }

    public int IdBloque { get; set; }

    public string? Abreviatura { get; set; }

    public string? TipoTiempos { get; set; }

    public int PagosDefault { get; set; }

    public string? TipoDifEntrePagos { get; set; }

    public int CapturaNoCuentaActivo { get; set; }

    public int HorasParaReventa { get; set; }

    public int CapturaIbanActivo { get; set; }

    public int CapturaDniActivo { get; set; }

    public decimal MontoProtegido { get; set; }

    public decimal MontoMaxPorcProtegido { get; set; }

    public decimal PorcParaProtegido { get; set; }

    public string? RazonSocial { get; set; }

    public string? Rfc { get; set; }

    public int DiasMaximo1erPago { get; set; }

    public int DiasMaximoDifEntrePagos { get; set; }

    public string? DirectorComercial { get; set; }

    public string? EmailDirectorComercial { get; set; }

    public string? Subdirector { get; set; }

    public string? EmailSubdirector { get; set; }

    public int ValidarTarjetaBinNoPermitidoPos { get; set; }

    public decimal VentaMinimaBono { get; set; }

    public decimal PorcentajeComisionesMenor { get; set; }

    public decimal PorcentajeComisionesMayor { get; set; }

    public decimal GteCuotaSemanalVenta { get; set; }

    public decimal GteCobranzaMinima { get; set; }

    public decimal VentaMiminaBonoLD { get; set; }

    public decimal MontoSobreventa { get; set; }

    public decimal CuotaMinParaNoPerderBono { get; set; }

    public decimal CaidaViernes { get; set; }

    public decimal CaidaSabado { get; set; }

    public decimal CaidaDomingo { get; set; }

    public decimal PotencialVenta { get; set; }

    public string? Gerente { get; set; }

    public int HorarioEspecial { get; set; }

    public int HorarioEspecialDiferenteRecepcion { get; set; }

    public int DiasMaximosPagosDir { get; set; }

    public int DiasMaximosPagosContraloria { get; set; }

    public int MaxPagosVerificador { get; set; }

    public int MaxPagosV1 { get; set; }

    public int MaxPagosV2 { get; set; }

    public int DiasMaximoPagoV1 { get; set; }

    public int DiasMaximoPagoV2 { get; set; }

    public string? EmailsNominaEnfermera { get; set; }

    public int CapturaIbanErroneo { get; set; }

    public int PermitirEnvioSMSAppWeb { get; set; }

    public int AnticipoMininoSinIBAN { get; set; }

    public string? CiudadSucursal { get; set; }

    public string? MonstrarMensajeDeActualizacionPendiente { get; set; }

    public bool UsarConfiguracionDePagosBancomer { get; set; }

    public bool PermitirCitaPaqueteConNegrita { get; set; }

    public DateTime? GioInicioTurno1 { get; set; }

    public DateTime? GioInicioTurno2 { get; set; }

    public DateTime? VendedorHoraCheckin { get; set; }

    public string? EmailsNotificacionToken { get; set; }

    public string? IdUsuariosSlackToken { get; set; }

    public int DirectorComercialIdUsuarioSlack { get; set; }

    public int SubdirectorIdUsuarioSlack { get; set; }

    public string? PosProcesadorCobrosDefault { get; set; }

    public string? ReciboLeyenda1 { get; set; }

    public string? ReciboLeyenda2 { get; set; }

    public decimal NoSesionesDefault { get; set; }

    public decimal MontoMaximoCobro { get; set; }

    public bool ValidarBinTarjetaAnticipoMinimo { get; set; }

    public decimal ValidarBinTarjetaAnticipoMinimoMonto { get; set; }

    public bool PermitirSolicitarHasta6pagosPorToken { get; set; }

    public string? EmailsTokenPermitirSolicitarHasta6pagos { get; set; }

    public string? IdUsuariosSlackTokenPermitirSolicitarHasta6pagos { get; set; }

    public decimal VentaMinimaPaquete { get; set; }

    public decimal ReventaMinimaPaquete { get; set; }

    public bool ValidarAreasMaxVentasPorCliente { get; set; }

    public string? EmailsTokenReventaEnCero { get; set; }

    public string? IdUsuariosSlackTokenReventaEnCero { get; set; }

    public bool AplicaPreciosBajos { get; set; }

    public decimal CostoMaxVentaNuevaParaAplicarAnticipoPreciosBajos { get; set; }

    public decimal PacienteMinPagadoParaAnticipoReventaCero { get; set; }

    public decimal PacienteMinPagadoHoyParaAnticipoReventaCero { get; set; }

    public bool PermitirReventaSinAnticipoMinimo { get; set; }

    public int TiempoAdicionalPrimeraCita { get; set; }

    public int TiempoMinimoPrimeraCita { get; set; }

    public bool SesionExtendida { get; set; }

    public int AumentoSesionExtendida { get; set; }

    public decimal AumentoPorcSesionExtendida { get; set; }

    public int TiempoMinimoSesionExtendida { get; set; }

    public string? UrlWebTokenizacionConekta { get; set; }

    public bool PermitirTokenizacionConekta { get; set; }

    public string? Nit { get; set; }

    public int EsEuroskin { get; set; }

    public int EsCredibanco { get; set; }

    public int BloqueoPos { get; set; }

    public int BloqueoPosAgenda { get; set; }

    public int EsEfevoo { get; set; }

    public string? EfevooDeviceId { get; set; }

    public string? EfevooSecret { get; set; }

    public int ValidaToku { get; set; }

    public string? TerminalIdMercadoPago { get; set; }
}
