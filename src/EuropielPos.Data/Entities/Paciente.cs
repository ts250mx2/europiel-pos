using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class Paciente
{
    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public int? IdPaciente { get; set; }

    public string? Nombre { get; set; }

    public string? ApPaterno { get; set; }

    public string? ApMaterno { get; set; }

    public string? Telefono1 { get; set; }

    public string? Telefono2 { get; set; }

    public string? Email { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdSucursal2 { get; set; }

    public DateTime? FechaAlta { get; set; }

    public int? IdUsuarioAlta { get; set; }

    public int? ClienteRobustoDs { get; set; }

    public int? ClienteRobustoS { get; set; }

    public string? Estatus { get; set; }

    public string? Domicilio { get; set; }

    public string? Colonia { get; set; }

    public string? CodigoPostal { get; set; }

    public string? Identidad { get; set; }

    public string? TipoIdentificacionCliente { get; set; }

    public bool? ClienteTieneCitaFuturo { get; set; }

    public bool? ClienteTiene1ercita { get; set; }

    public string? Identidad2 { get; set; }

    public int? IdTipoIdentificacion2 { get; set; }

    public bool? CargosAutoUltimos6meses { get; set; }

    public decimal? TotalPagado { get; set; }

    public decimal? SaldoVencidoPaciente5Dias { get; set; }

    public decimal? SaldoVencidoPaciente { get; set; }

    public decimal? SaldoTotalPaciente { get; set; }

    public string? Sexo { get; set; }

    public DateTime? FechaNacimiento { get; set; }

    public int? Edad { get; set; }

    public int? IdPais { get; set; }

    public string? Estado { get; set; }

    public string? Municipio { get; set; }

    public string? Num { get; set; }

    public int? IdTipoIdentificacion { get; set; }

    public int? IdCiudad { get; set; }

    public int? IdEstado { get; set; }
}
