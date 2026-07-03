namespace EuropielPos.Domain.Paquetes;

/// <summary>
/// Datos para actualizar una reventa de paquete (SP <c>actualiza_reventa_paquete</c>).
/// El BL original recibía estos 41 valores como parámetros sueltos; se agrupan
/// aquí 1:1 con los parámetros del stored procedure.
/// Un paquete admite hasta 10 pagos programados y hasta 5 pacientes.
/// </summary>
public class ReventaPaquete
{
    public int IdPaqueteLocal { get; set; }

    public decimal Anticipo { get; set; }

    public decimal CostoTotal { get; set; }

    public string TokenT1 { get; set; } = string.Empty;

    public int PagosXCubrir { get; set; }

    public decimal PagoUnitario { get; set; }

    public DateTime FechaPago1 { get; set; }

    public DateTime FechaPago2 { get; set; }

    public DateTime FechaPago3 { get; set; }

    public DateTime FechaPago4 { get; set; }

    public DateTime FechaPago5 { get; set; }

    public DateTime FechaPago6 { get; set; }

    public DateTime FechaPago7 { get; set; }

    public DateTime FechaPago8 { get; set; }

    public DateTime FechaPago9 { get; set; }

    public DateTime FechaPago10 { get; set; }

    public decimal MontoPago1 { get; set; }

    public decimal MontoPago2 { get; set; }

    public decimal MontoPago3 { get; set; }

    public decimal MontoPago4 { get; set; }

    public decimal MontoPago5 { get; set; }

    public decimal MontoPago6 { get; set; }

    public decimal MontoPago7 { get; set; }

    public decimal MontoPago8 { get; set; }

    public decimal MontoPago9 { get; set; }

    public decimal MontoPago10 { get; set; }

    public int IdPaciente { get; set; }

    public int IdPacienteLocal { get; set; }

    public string NombrePaciente1 { get; set; } = string.Empty;

    public int IdPaciente2 { get; set; }

    public int IdPacienteLocal2 { get; set; }

    public string NombrePaciente2 { get; set; } = string.Empty;

    public int IdPaciente3 { get; set; }

    public int IdPacienteLocal3 { get; set; }

    public string NombrePaciente3 { get; set; } = string.Empty;

    public int IdPaciente4 { get; set; }

    public int IdPacienteLocal4 { get; set; }

    public string NombrePaciente4 { get; set; } = string.Empty;

    public int IdPaciente5 { get; set; }

    public int IdPacienteLocal5 { get; set; }

    public string NombrePaciente5 { get; set; } = string.Empty;
}
