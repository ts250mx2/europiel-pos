using EuropielPos.Data.Services;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas del cálculo de duración de citas (lógica pura de frmCitaDetalle).
/// </summary>
public class NuevaCitaServiceTests
{
    private static NuevaCitaService Crear() => new(null!, new(), null!); // solo lógica pura

    private static AreaAgendable Area(decimal duracion, decimal servicio = 0, bool seleccionada = true) =>
        new() { Duracion = duracion, DuracionServicio = servicio, Seleccionada = seleccionada };

    [Fact]
    public void CalculaDuracion_SumaDuracionesMasMaximoServicio()
    {
        var servicio = Crear();
        var config = new ConfigAgendado();
        var lunes = new DateTime(2026, 7, 6);

        // 20 + 30 de áreas + max(10, 15) de servicio = 65
        int duracion = servicio.CalculaDuracion(
            [Area(20, 10), Area(30, 15)], config, lunes);

        Assert.Equal(65, duracion);
    }

    [Fact]
    public void CalculaDuracion_SesionExtendida_SoloEntreSemana()
    {
        var servicio = Crear();
        var config = new ConfigAgendado
        {
            SesionExtendida = true,
            AumentoSesionExtendida = 10,
            AumentoPorcSesionExtendida = 1.5m,
            TiempoMinimoSesionExtendida = 30,
        };

        var miercoles = new DateTime(2026, 7, 8);
        var sabado = new DateTime(2026, 7, 11);

        // Entre semana: ceil((20+10)*1.5) = 45
        Assert.Equal(45, servicio.CalculaDuracion([Area(20)], config, miercoles));
        // Sábado: sin ajuste = 20
        Assert.Equal(20, servicio.CalculaDuracion([Area(20)], config, sabado));
    }

    [Fact]
    public void CalculaDuracion_PrimeraCita_AgregaTiempoYRespetaMinimo()
    {
        var servicio = Crear();
        var config = new ConfigAgendado
        {
            ClienteTiene1erCita = false, // es su primera cita
            TiempoAdicionalPrimeraCita = 15,
            TiempoMinimoPrimeraCita = 60,
        };
        var lunes = new DateTime(2026, 7, 6);

        // 20 + 15 adicional = 35, pero el mínimo de primera cita es 60
        Assert.Equal(60, servicio.CalculaDuracion([Area(20)], config, lunes));

        // 50 + 15 = 65 ya supera el mínimo
        Assert.Equal(65, servicio.CalculaDuracion([Area(50)], config, lunes));
    }

    [Fact]
    public void CalculaDuracion_SinSeleccion_EsCero()
    {
        var servicio = Crear();

        Assert.Equal(0, servicio.CalculaDuracion([], new ConfigAgendado(), DateTime.Today));
    }
}
