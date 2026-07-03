using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Sucursal
{
    public int IdSucursal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public string? Descripcion { get; set; }

    public int? EsActiva { get; set; }

    public int? IdGerente { get; set; }

    public decimal? VentaMinimaBono { get; set; }

    public decimal? PorcentajeComisionesMenor { get; set; }

    public decimal? PorcentajeComisionesMayor { get; set; }

    public decimal? VentaMiminaBonoLd { get; set; }

    public decimal? GteCuotaSemanalVenta { get; set; }

    public decimal? GteCobranzaMinima { get; set; }

    public decimal? MontoSobreventa { get; set; }

    public decimal? CuotaMinParaNoPerderBono { get; set; }

    public string? StoreidNetpay { get; set; }

    public int? IdGerenteApoyo { get; set; }

    public string? Timezone { get; set; }

    public string? Prefijo { get; set; }

    public string? Direccion { get; set; }

    public string? StoreidTerminalInterfaz { get; set; }

    public string? UsernameTerminalInterfaz { get; set; }

    public string? PasswordTerminalInterfaz { get; set; }

    public int? CantidadCitasEmpalmadas { get; set; }

    public int? HorarioDiferenteRecepcion { get; set; }

    public DateTime? AgendaHoraInicio { get; set; }

    public DateTime? AgendaHoraFin { get; set; }

    public int? IdBloque { get; set; }

    public string? TipoTiempos { get; set; }

    public string? DirectorComercial { get; set; }

    public string? EmailDirectorComercial { get; set; }

    public string? Subdirector { get; set; }

    public string? EmailSubdirector { get; set; }

    public int? DirectorComercialIdUsuarioSlack { get; set; }

    public int? SubdirectorIdUsuarioSlack { get; set; }

    public string? EmailsNotificacionToken { get; set; }

    public string? IdUsuariosSlackToken { get; set; }

    public decimal? CaidaViernes { get; set; }

    public decimal? CaidaSabado { get; set; }

    public decimal? CaidaDomingo { get; set; }

    public decimal? PotencialVenta { get; set; }

    public string? Gerente { get; set; }

    public DateTime? GioInicioTurno1 { get; set; }

    public DateTime? GioInicioTurno2 { get; set; }

    public DateTime? VendedorHoraCheckin { get; set; }

    public int? HorarioEspecial { get; set; }

    public int? HorarioEspecialDiferenteRecepcion { get; set; }

    public int? IdPais { get; set; }

    public string? CiudadSucursal { get; set; }

    public string? TelefonoSucursal { get; set; }

    public bool? SesionExtendida { get; set; }

    public int? TiempoAdicionalPrimeraCita { get; set; }

    public int? TiempoMinimoPrimeraCita { get; set; }

    public int? AumentoSesionExtendida { get; set; }

    public decimal? AumentoPorcSesionExtendida { get; set; }

    public int? TiempoMinimoSesionExtendida { get; set; }

    public bool? AplicaPreciosBajos { get; set; }

    public decimal? CostoMaxVentaNuevaParaAplicarAnticipoPreciosBajos { get; set; }

    public decimal? PacienteMinPagadoParaAnticipoReventaCero { get; set; }

    public decimal? PacienteMinPagadoHoyParaAnticipoReventaCero { get; set; }

    public bool? PermitirReventaSinAnticipoMinimo { get; set; }

    public string? EmailsTokenReventaEnCero { get; set; }

    public string? IdUsuariosSlackTokenReventaEnCero { get; set; }

    public bool? ValidarAreasMaxVentasPorCliente { get; set; }

    public bool? PermitirSolicitarHasta6pagosXToken { get; set; }

    public string? EmailsTokenPermitirSolicitarHasta6pagos { get; set; }

    public string? IdUsuariosSlackTokenPermitirSolicitarHasta6pagos { get; set; }

    public bool? ValidarBinTarjetaAnticipoMinimo { get; set; }

    public decimal? ValidarBinTarjetaAnticipoMinimoMonto { get; set; }

    public bool? PermitirTokenizacionConekta { get; set; }

    public string? UrlWebTokenizacionConekta { get; set; }

    public int? EsCredibanco { get; set; }

    public int? EsEfevoo { get; set; }

    public string? EfevooDeviceId { get; set; }

    public string? EfevooSecret { get; set; }

    public int? ValidaToku { get; set; }

    public string? TerminalIdMercadoPago { get; set; }

    public virtual ICollection<Documento> Documento { get; set; } = new List<Documento>();

    public virtual ICollection<ServicioSucursal> ServicioSucursal { get; set; } = new List<ServicioSucursal>();

    public virtual ICollection<SucursalAgendaExterna> SucursalAgendaExternaIdSucursalAfinNavigation { get; set; } = new List<SucursalAgendaExterna>();

    public virtual ICollection<SucursalAgendaExterna> SucursalAgendaExternaIdSucursalNavigation { get; set; } = new List<SucursalAgendaExterna>();
}
