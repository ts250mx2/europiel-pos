using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Paquete
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdPacienteLocal { get; set; }

    public int? IdPaciente { get; set; }

    public int? IdPacienteLocal2 { get; set; }

    public int? IdPaciente2 { get; set; }

    public string? NombrePaciente1 { get; set; }

    public string? NombrePaciente2 { get; set; }

    public DateTime? FechaCompra { get; set; }

    public bool? TratLaser { get; set; }

    public bool? TratCorporal { get; set; }

    public bool? TratFacial { get; set; }

    public string? FormaPago { get; set; }

    public string? TipoCobranza { get; set; }

    public string? TarjetaNumero { get; set; }

    public string? TarjetaTipo { get; set; }

    public string? TarjetaCvv { get; set; }

    public string? TarjetaFechaVenc { get; set; }

    public decimal? CostoTotal { get; set; }

    public decimal? Anticipo { get; set; }

    public int? PagosXCubrir { get; set; }

    public decimal? PagoUnitario { get; set; }

    public DateTime? FechaPago1 { get; set; }

    public DateTime? FechaPago2 { get; set; }

    public DateTime? FechaPago3 { get; set; }

    public DateTime? FechaPago4 { get; set; }

    public DateTime? FechaPago5 { get; set; }

    public DateTime? FechaPago6 { get; set; }

    public DateTime? FechaPago7 { get; set; }

    public DateTime? FechaPago8 { get; set; }

    public DateTime? FechaPago9 { get; set; }

    public DateTime? FechaPago10 { get; set; }

    public decimal? MontoPago1 { get; set; }

    public decimal? MontoPago2 { get; set; }

    public decimal? MontoPago3 { get; set; }

    public decimal? MontoPago4 { get; set; }

    public decimal? MontoPago5 { get; set; }

    public decimal? MontoPago6 { get; set; }

    public decimal? MontoPago7 { get; set; }

    public decimal? MontoPago8 { get; set; }

    public decimal? MontoPago9 { get; set; }

    public decimal? MontoPago10 { get; set; }

    public string? Observaciones { get; set; }

    public int? IdUsuario { get; set; }

    public DateTime? FechaAlta { get; set; }

    public bool? TratLaser2 { get; set; }

    public bool? TratCorporal2 { get; set; }

    public bool? TratFacial2 { get; set; }

    public int? IdSucursal { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? IdUsuarioModifica { get; set; }

    public bool? PagadoMitad { get; set; }

    public DateTime? PagadoMitadFecha { get; set; }

    public bool? MostrarEnSucursales { get; set; }

    public string? TipoCobranza1 { get; set; }

    public string? TipoCobranza2 { get; set; }

    public string? TipoCobranza3 { get; set; }

    public string? TipoCobranza4 { get; set; }

    public string? TipoCobranza5 { get; set; }

    public string? TipoCobranza6 { get; set; }

    public string? TipoCobranza7 { get; set; }

    public string? TipoCobranza8 { get; set; }

    public string? TipoCobranza9 { get; set; }

    public string? TipoCobranza10 { get; set; }

    public int? IdPromocion { get; set; }

    public decimal? CostoTotalCalculado { get; set; }

    public bool? ClienteReferido { get; set; }

    public int? IdClienteRefirio { get; set; }

    public bool? TratamientoGratisReferido { get; set; }

    public int? IdClienteReferido { get; set; }

    public int? IdMedio { get; set; }

    public int? IdSucursalOrigen { get; set; }

    public DateTime? FechaCobranzaAutomatica { get; set; }

    public bool? EsReventa { get; set; }

    public bool? NoDisponiblePorMigracion { get; set; }

    public bool? BorradoEnMigracion { get; set; }

    public bool? ProvieneDeMigracion { get; set; }

    public DateTime? FechaRecuperacionCobranza { get; set; }

    public DateTime? FechaCitaUltimatum { get; set; }

    public string? TarjetaCs { get; set; }

    public string? TarjetaNumeroT2 { get; set; }

    public string? TarjetaTipoT2 { get; set; }

    public string? TarjetaCvvT2 { get; set; }

    public string? TarjetaFechaVencT2 { get; set; }

    public string? TarjetaCsT2 { get; set; }

    public int? TarjetaPrimaria { get; set; }

    public bool? PaqueteCompleto { get; set; }

    public bool? CompartidoEnfermera { get; set; }

    public int? IdEnfermera { get; set; }

    public int? IdPacienteLocal3 { get; set; }

    public int? IdPaciente3 { get; set; }

    public string? NombrePaciente3 { get; set; }

    public bool? TratLaser3 { get; set; }

    public bool? TratCorporal3 { get; set; }

    public bool? TratFacial3 { get; set; }

    public decimal? SaldoVencido { get; set; }

    public decimal? SaldoVencido5Dias { get; set; }

    public decimal? SaldoTotal { get; set; }

    public bool? EsNegrita { get; set; }

    public DateTime? FechaActualizacionSaldos { get; set; }

    public int? SiEnviarPaqueteServicio { get; set; }

    public string? TokenT1 { get; set; }

    public string? TokenT2 { get; set; }

    public string? Contrato { get; set; }

    public DateTime? FechaSigtePago { get; set; }

    public DateTime? FechaNegrita { get; set; }

    public int? NumPagoSaldoVencido { get; set; }

    public int? PerdidaGarantia { get; set; }

    public bool? VentaCompletaEnfermera { get; set; }

    public int? IdUsuarioVtaComEnf { get; set; }

    public DateTime? FechaVtaComEnf { get; set; }

    public int? CantPagosSaldoVencido { get; set; }

    public int? IdPacienteLocal4 { get; set; }

    public int? IdPaciente4 { get; set; }

    public string? NombrePaciente4 { get; set; }

    public bool? TratLaser4 { get; set; }

    public bool? TratCorporal4 { get; set; }

    public bool? TratFacial4 { get; set; }

    public int? IdPacienteLocal5 { get; set; }

    public int? IdPaciente5 { get; set; }

    public string? NombrePaciente5 { get; set; }

    public bool? TratLaser5 { get; set; }

    public bool? TratCorporal5 { get; set; }

    public bool? TratFacial5 { get; set; }

    public bool? SugerirTc1 { get; set; }

    public string? NoCuenta { get; set; }

    public string? Iban { get; set; }

    public string? Dni { get; set; }

    public DateTime? FechaCapturaIban { get; set; }

    public bool? IbanErroneo { get; set; }

    public string? FirmaContrato { get; set; }

    public string? TipoCuenta { get; set; }

    public bool? TarjetaNoPudoValidarCauto { get; set; }

    public bool? CobrableConekta { get; set; }

    public int? ConektaPromocionMsi { get; set; }

    public int? IdBanco { get; set; }

    public int? IdBanco2 { get; set; }

    public int? EsEuroskin { get; set; }

    public virtual ICollection<FeeniciaSubscripciones> FeeniciaSubscripciones { get; set; } = new List<FeeniciaSubscripciones>();
}
