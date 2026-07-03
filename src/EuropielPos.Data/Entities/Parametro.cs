using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Parametro
{
    public int Id { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public string? EmailsBloqueos { get; set; }

    public decimal? DescMax { get; set; }

    public decimal? DescMaxSueltas { get; set; }

    public decimal? FactorDescuentoPaquetes { get; set; }

    public decimal? DescMaxMonto { get; set; }

    public bool? DescTipoMonto { get; set; }

    public int? MaxPagosVendedor { get; set; }

    public int? DiasMaximoPagos { get; set; }

    public string? Idioma { get; set; }

    public string? Bloque { get; set; }

    public decimal? AnticipoMinimo { get; set; }

    public decimal? AnticipoMinimoCauto { get; set; }

    public string? EmailsNotificacionPaqCostoInferior { get; set; }

    public int? ClaveBloque { get; set; }

    public string? NombreEmpresa { get; set; }

    public string? RepresentanteLegal { get; set; }

    public string? Moneda { get; set; }

    public int? IdSucursal { get; set; }

    public int? NoCaja { get; set; }

    public string? PuertoTerminalNetpay { get; set; }

    public string? TerminalIdNetpay { get; set; }

    public string? VersionSistema { get; set; }

    public string? ImpresoraVoucher { get; set; }

    public int? PagosDefault { get; set; }

    public string? TipoDifEntrePagos { get; set; }

    public int? CapturaNoCuentaActivo { get; set; }

    public int? CapturaIbanActivo { get; set; }

    public int? HorasParaReventa { get; set; }

    public int? CapturaDniActivo { get; set; }

    public decimal? MontoProtegido { get; set; }

    public decimal? MontoMaxPorcProtegido { get; set; }

    public decimal? PorcParaProtegido { get; set; }

    public string? RazonSocial { get; set; }

    public string? Rfc { get; set; }

    public int? DiasMaximo1erPago { get; set; }

    public int? DiasMaximoDifEntrePagos { get; set; }

    public decimal? DifVentasSemanaPasada { get; set; }

    public int? ValidarTarjetaBinNoPermitidoPos { get; set; }

    public string? ReciboLeyenda1 { get; set; }

    public string? ReciboLeyenda2 { get; set; }

    public string? PosProcesadorCobrosDefault { get; set; }

    public string? TerminalBsdIp { get; set; }

    public int? DiasMaximoPagosDir { get; set; }

    public int? DiasMaximoPagosContraloria { get; set; }

    public int? MaxPagosVerificador { get; set; }

    public int? MaxPagosV1 { get; set; }

    public int? MaxPagosV2 { get; set; }

    public int? DiasMaximoPagosV1 { get; set; }

    public int? DiasMaximoPagosV2 { get; set; }

    public string? EmailsNominaEnfermera { get; set; }

    public int? CapturaIbanErroneo { get; set; }

    public int? PermitirEnvioSmsAppWeb { get; set; }

    public decimal? AnticipoMinimoSinIban { get; set; }

    public bool? PermitirCitaPaqueteConNegrita { get; set; }

    public bool? UsarConfiguracionDePagosBancomer { get; set; }

    public string? CcRepresentanteLegal { get; set; }

    public decimal? AnticipoMinimoReventa { get; set; }

    public string? Abreviatura { get; set; }

    public decimal? VentaMinimaPaquete { get; set; }

    public decimal? ReventaMinimaPaquete { get; set; }

    public decimal? MontoMaximoCobro { get; set; }

    public int? NoSesionesDefault { get; set; }

    public string? Nit { get; set; }

    public int? EsEuroskin { get; set; }

    public int? BloqueoPos { get; set; }

    public int? BloqueoPosAgenda { get; set; }

    public int? EsCredibanco { get; set; }

    public int? EsEfevoo { get; set; }

    public string? EfevooDeviceId { get; set; }

    public string? EfevooSecret { get; set; }

    public int? ValidaToku { get; set; }

    public string? TerminalIdMercadoPago { get; set; }
}
