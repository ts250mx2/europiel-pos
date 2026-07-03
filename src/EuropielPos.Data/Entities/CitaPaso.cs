using System;
using System.Collections.Generic;

namespace EuropielPos.Data.Entities;

public partial class CitaPaso
{
    public int Id { get; set; }

    public int IdLocal { get; set; }

    public DateTime? FechaModificacionLocal { get; set; }

    public DateTime? FechaInterfaz { get; set; }

    public string? EstatusInterfaz { get; set; }

    public int IdCita { get; set; }

    public int? NumCita { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? IdPaciente { get; set; }

    public int? IdPacienteLocal { get; set; }

    public int? IdMedico { get; set; }

    public int? IdTipoCita { get; set; }

    public string? Estatus { get; set; }

    public DateTime? FechaAlta { get; set; }

    public int? UsuarioAlta { get; set; }

    public DateTime? FechaEstatus { get; set; }

    public int? UsuarioEstatus { get; set; }

    public bool? NoSePago { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdServicio { get; set; }

    public int? IdUsuarioServicio { get; set; }

    public DateTime? FechaUsuarioCaptura { get; set; }

    public int? IdPaquete { get; set; }

    public int? IdPaqueteLocal { get; set; }

    public int? Asistida { get; set; }

    public int? IdPadre { get; set; }

    public int? IdPadreLocal { get; set; }

    public int? CitasRealmenteAsistidas { get; set; }

    public string? AltaCita { get; set; }

    public string? ListaServicios { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? EsMorada { get; set; }

    public int? IdRemotoInterfaz { get; set; }

    public int? IdSucursalInterfaz { get; set; }

    public int? NoCajaInterfaz { get; set; }

    public int? ReimprimirContrato { get; set; }

    public int? ClienteAsistio { get; set; }

    public bool? EsConsultaMedica { get; set; }

    public DateTime? FechaConsultaInterfaz { get; set; }
}
