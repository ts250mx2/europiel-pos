namespace EuropielPos.Domain.Gios;

/// <summary>
/// Port de <c>RecuperaFechaParaCheckin</c> de MDIForm.vb (rama de check-in
/// manual): a qué turno se abona el check-in facial que se está registrando.
/// </summary>
public static class FechaCheckin
{
    /// <summary>Regla para GIOs: si falta una hora o menos para el turno 2 (o
    /// ya pasó), el check-in cuenta para el turno 2; si no, para el turno 1.
    /// Sin turnos configurados cuenta al momento actual.</summary>
    public static DateTime ParaCheckinManualGio(DateTime ahora, DateTime? inicioTurno1, DateTime? inicioTurno2)
    {
        if (inicioTurno1 is null && inicioTurno2 is null)
            return ahora;

        if (inicioTurno2 is not null && ahora > inicioTurno2.Value.AddHours(-1))
            return inicioTurno2.Value;

        // El original devolvía GioInicioTurno1 aunque viniera vacío (habría
        // tronado al usarlo); aquí se cae al momento actual.
        return inicioTurno1 ?? ahora;
    }

    /// <summary>Regla para vendedores: la hora configurada o el momento actual.</summary>
    public static DateTime ParaCheckinManualVendedor(DateTime ahora, DateTime? horaCheckin) =>
        horaCheckin ?? ahora;

    /// <summary>Como frmLogin: convierte la hora configurada en la sucursal
    /// (guardada con fecha arbitraria) a la fecha de hoy.</summary>
    public static DateTime? HoraDeHoy(DateTime? horaConfigurada, DateTime hoy) =>
        horaConfigurada is null
            ? null
            : new DateTime(hoy.Year, hoy.Month, hoy.Day,
                horaConfigurada.Value.Hour, horaConfigurada.Value.Minute, horaConfigurada.Value.Second);
}
