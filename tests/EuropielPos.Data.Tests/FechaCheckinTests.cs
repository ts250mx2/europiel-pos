using EuropielPos.Domain.Gios;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de la regla de RecuperaFechaParaCheckin (MDIForm.vb): a qué turno
/// se abona un check-in manual.
/// </summary>
public class FechaCheckinTests
{
    private static readonly DateTime Turno1 = new(2026, 7, 7, 9, 0, 0);
    private static readonly DateTime Turno2 = new(2026, 7, 7, 15, 0, 0);

    [Fact]
    public void SinTurnosConfigurados_DevuelveAhora()
    {
        var ahora = new DateTime(2026, 7, 7, 10, 30, 0);

        Assert.Equal(ahora, FechaCheckin.ParaCheckinManualGio(ahora, null, null));
    }

    [Fact]
    public void AntesDeLaVentanaDelTurno2_CuentaAlTurno1()
    {
        // 13:59 < 14:00 (turno2 - 1h) → turno 1.
        var ahora = new DateTime(2026, 7, 7, 13, 59, 0);

        Assert.Equal(Turno1, FechaCheckin.ParaCheckinManualGio(ahora, Turno1, Turno2));
    }

    [Fact]
    public void DentroDeLaVentanaDelTurno2_CuentaAlTurno2()
    {
        // 14:01 > 14:00 (turno2 - 1h) → turno 2.
        var ahora = new DateTime(2026, 7, 7, 14, 1, 0);

        Assert.Equal(Turno2, FechaCheckin.ParaCheckinManualGio(ahora, Turno1, Turno2));
    }

    [Fact]
    public void SoloTurno1Configurado_CuentaAlTurno1()
    {
        var ahora = new DateTime(2026, 7, 7, 18, 0, 0);

        Assert.Equal(Turno1, FechaCheckin.ParaCheckinManualGio(ahora, Turno1, null));
    }

    [Fact]
    public void Vendedor_UsaHoraConfiguradaOAhora()
    {
        var ahora = new DateTime(2026, 7, 7, 10, 0, 0);
        var hora = new DateTime(2026, 7, 7, 9, 30, 0);

        Assert.Equal(hora, FechaCheckin.ParaCheckinManualVendedor(ahora, hora));
        Assert.Equal(ahora, FechaCheckin.ParaCheckinManualVendedor(ahora, null));
    }

    [Fact]
    public void HoraDeHoy_CombinaHoraConfiguradaConFechaActual()
    {
        // La sucursal guarda la hora con una fecha arbitraria vieja.
        var configurada = new DateTime(1900, 1, 1, 15, 30, 45);
        var hoy = new DateTime(2026, 7, 7, 11, 0, 0);

        Assert.Equal(new DateTime(2026, 7, 7, 15, 30, 45), FechaCheckin.HoraDeHoy(configurada, hoy));
        Assert.Null(FechaCheckin.HoraDeHoy(null, hoy));
    }
}
